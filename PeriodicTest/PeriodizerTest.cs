using Xunit;
using Periodic;
using System;
using System.Linq;
using Periodic.Ts;

namespace PeriodicTest
{
    public class PeriodizerTest
    {
        Tvqs m_Tvqs;
        private Periodizer m_Periodizer;

        public PeriodizerTest()
        {
            m_Tvqs = new Tvqs();
            m_Periodizer = new Periodizer();
        }

        [Fact]
        public void HasEmptyIntervalInBetween_WhenFirstOfJanuaryAndMarch_ReturnsTrue()
        {
            Assert.True(m_Periodizer.HasEmptyIntervalInBetween(m_Tvqs.Tvq20150101.Time, m_Tvqs.Tvq20150301.Time, Interval.Month));
        }

        [Fact]
        public void HasEmptyIntervalInBetween_WhenLastOfDecemberAndFirstOfMarch_ReturnsTrue()
        {
            Assert.True(m_Periodizer.HasEmptyIntervalInBetween(m_Tvqs.Tvq20151231.Time, m_Tvqs.Tvq20160201.Time, Interval.Month));
        }

        [Fact]
        public void HasEmptyIntervalInBetween_WhenBothJanuary_ReturnsFalse()
        {
            Assert.False(m_Periodizer.HasEmptyIntervalInBetween(m_Tvqs.Tvq20150101.Time, m_Tvqs.Tvq20150105.Time, Interval.Month));
        }

        [Fact]
        public void HasEmptyIntervalInBetween_WhenBothDecember_ReturnsFalse()
        {
            Assert.False(m_Periodizer.HasEmptyIntervalInBetween(m_Tvqs.Tvq20151201.Time, m_Tvqs.Tvq20151231.Time, Interval.Month));
        }

        [Fact]
        public void InsertPoints_WhenNullThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => m_Periodizer.InsertPoints(null, Interval.Year));
        }

        [Fact]
        public void InsertPoints_WhenEmptyTs_ReturnsEmptyTs()
        {
            Assert.Equal(0, m_Periodizer.InsertPoints(new Timeseries(), Interval.Year).Count);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointAtLastSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150101};
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);
            var tvq = result.Last();
            Assert.Equal(new DateTime(2015, 12, 31, 23, 59, 59), tvq.Time);
        }

        [Fact]
        public void InsertPoints_WhenMonthOnePointAtStart_InsertsEndpointAtLastSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150101};
            var result = m_Periodizer.InsertPoints(ts, Interval.Month);
            var tvq = result.Last();
            Assert.Equal(new DateTime(2015, 01, 31, 23, 59, 59), tvq.Time);
        }
        
        [Fact]
        public void InsertPoints_WhenMonthOnePointAtStart_InsertsEndpointWithSameValueAsInterpolated()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20150101 };
            var result = m_Periodizer.InsertPoints(ts, Interval.Month);
            var tvq = result.Last();
            Assert.Equal(ts[0].V, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointWithSameValueAsInterpolated()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20150101 };
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);
            var tvq = result.Last();
            Assert.Equal(ts[0].V, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenOnePoint_InsertsStartpointAtFirstSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150601};
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);
            var tvq = result.First();
            Assert.Equal(new DateTime(2015, 01, 01, 0, 0, 0), tvq.Time);
        }

        [Fact]
        public void InsertPoints_WhenMonthOnePoint_InsertsStartpointAtFirstSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150622};
            var result = m_Periodizer.InsertPoints(ts, Interval.Month);
            var tvq = result.First();
            Assert.Equal(new DateTime(2015, 06, 01, 0, 0, 0), tvq.Time);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointWithSameValue()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150101};
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);
            Assert.Equal(2, result.Count);
            var tvq = result.Last();
            Assert.Equal(m_Tvqs.Tvq20150101.V, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtEnd_DoesNotInsertEndpoint()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20151231 };
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);
            Assert.Equal(2, result.Count);
            var tvq = result.Last();
            Assert.Same(m_Tvqs.Tvq20151231, tvq);
        }

        [Fact]
        public void InsertPoints_WhenMonthOnePointAtEnd_DoesNotInsertEndpoint()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20150630 };
            var result = m_Periodizer.InsertPoints(ts, Interval.Month);
            Assert.Equal(2, result.Count);
            var tvq = result.Last();
            Assert.Same(m_Tvqs.Tvq20150630, tvq);
        }
        
        [Fact]
        public void InsertPoints_WhenTwoPointsSameYear_InsertsExtrapolatedEndValue()
        {
            // Arrange
            var ts = new Timeseries {m_Tvqs.Tvq20150101, m_Tvqs.Tvq20150601};

            // Act
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);

            // Assert
            Assert.Equal(3, result.Count);
            var dt = result[1].Time - result[0].Time;
            // y = k*x + m
            var k = (m_Tvqs.Tvq20150601.V - m_Tvqs.Tvq20150101.V) / dt.TotalSeconds;
            var t1 = new DateTime(2015, 12, 31, 23, 59, 59);
            var x = (t1 - m_Tvqs.Tvq20150601.Time).TotalSeconds;
            var y = k * x + m_Tvqs.Tvq20150601.V;
            var tvq = result.Last();
            Assert.Equal(t1, tvq.Time);
            Assert.Equal(y, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenTwoPointsSameMonth_InsertsExtrapolatedEndValue()
        {
            // Arrange
            var tvq1 = m_Tvqs.Tvq20150601;
            var tvq2 = m_Tvqs.Tvq20150622;
            var ts = new Timeseries {tvq1, tvq2};

            // Act
            var result = m_Periodizer.InsertPoints(ts, Interval.Month);

            // Assert
            Assert.Equal(3, result.Count);
            var dt = result[1].Time - result[0].Time;
            // y = k*x + m
            var k = (tvq2.V - tvq1.V) / dt.TotalSeconds;
            var t1 = m_Tvqs.Tvq20150630.Time;
            var x = (t1 - tvq1.Time).TotalSeconds;
            var y = k * x + tvq1.V;
            var tvq = result.Last();
            Assert.Equal(t1, tvq.Time);
            Assert.Equal(y, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_When3PointsSameYear_InsertsExtrapolatedEndValue()
        {
            // Arrange
            var ts = new Timeseries { m_Tvqs.Tvq20150101, m_Tvqs.Tvq20150601, m_Tvqs.Tvq20150701 };

            // Act
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);

            // Assert
            Assert.Equal(4, result.Count);
            var dt = result[2].Time - result[1].Time;
            // y = k*x + m
            var k = (m_Tvqs.Tvq20150701.V - m_Tvqs.Tvq20150601.V) / dt.TotalSeconds;
            var t1 = new DateTime(2015, 12, 31, 23, 59, 59);
            var x = (t1 - m_Tvqs.Tvq20150701.Time).TotalSeconds;
            var y = k * x + m_Tvqs.Tvq20150701.V;
            var tvq = result.Last();
            Assert.Equal(t1, tvq.Time);
            Assert.Equal(y, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenTwoPointsAdjacentYears_InsertsInterpolatedValues()
        {
            // Arrange
            var ts = new Timeseries { m_Tvqs.Tvq20150101, m_Tvqs.Tvq20160601};

            // Act
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);

            // Assert
            Assert.Equal(5, result.Count);
            var dt = ts[1].Time - ts[0].Time;
            // y = k*x + m
            var k = (ts[1].V - ts[0].V) / dt.TotalSeconds;
            var t1 = new DateTime(2015, 12, 31, 23, 59, 59);
            var x = (t1 - ts[0].Time).TotalSeconds;
            var y = k * x + ts[0].V;
            var tvq1 = result[1];
            Assert.Equal(t1, tvq1.Time);
            Assert.Equal(y, tvq1.V);
            Assert.Equal(Quality.Interpolated, tvq1.Q);
        }

        [Fact]
        public void InsertPoints_WhenFrom2015To2016_TimesAreCorrect()
        {
            // Arrange
            // Act
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150601.Time, 4, Quality.Ok),
                new Tvq(m_Tvqs.Tvq20160601.Time, 4, Quality.Ok)
            };
            var averages = m_Periodizer.InsertPoints(ts, Interval.Month);

            //Assert
            var i = 0;
            Assert.Equal(new DateTime(2015, 06, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 06, 30, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 07, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 07, 31, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 08, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 08, 31, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 09, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 09, 30, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 10, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 10, 31, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 11, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 11, 30, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 12, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2015, 12, 31, 23, 59, 59), averages[i++].Time);
            Assert.Equal(new DateTime(2016, 01, 01, 0, 0, 0), averages[i++].Time);
            Assert.Equal(new DateTime(2016, 06, 30, 23, 59, 59), averages.Last().Time);
        }


        [Fact]
        public void MonthlyAverage_WhenNull_ReturnsEmpty()
        {
            Assert.Empty(m_Periodizer.MonthlyAverage(null));
        }

        [Fact]
        public void MonthlyAverage_WhenEmpty_ReturnsEmpty()
        {
            Assert.Empty(m_Periodizer.MonthlyAverage(new Timeseries()));
        }

        [Fact]
        public void MonthlyAverage_WhenOneValueFirstSecondInJan_Returns0()
        {
            // Arrange
            // Act
            var averages = m_Periodizer.MonthlyAverage(new Timeseries {m_Tvqs.Tvq20150101});

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            Assert.Equal(0, x.V);
        }

        [Fact]
        public void MonthlyAverage_WhenOneValueInJan_Returns0()
        {
            // Arrange
            // Act
            var averages = m_Periodizer.MonthlyAverage(new Timeseries { m_Tvqs.Tvq20150105 });

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            Assert.Equal(0, x.V, 9);
        }

        [Fact]
        public void MonthlyAverage_When2ValuesInMiddleOfJan_ReturnsAsAverageAtFirstSecond()
        {
            // Arrange
            const double v0 = 0;
            const double v1 = 500;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150105.Time, v0, Quality.Ok),
                new Tvq(m_Tvqs.Tvq20150110.Time, v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);

            var totalDays = (ts[1].Time - ts[0].Time).Days;
            var consumptionPerDay = (v1 - v0) / totalDays;

            var nDaysExtrapolated = 1 + 31 - ts[1].Time.Day;
            var extrapolatedConsumption = consumptionPerDay * nDaysExtrapolated;
            var valueAtEnd = v1 + extrapolatedConsumption;
            var expectedValue = valueAtEnd - v0;

            Assert.Equal(expectedValue, x.V, 4);
        }

        [Fact]
        public void MonthlyAverage_When1ValueAtStartOfJanAnd1InFeb_InterpolatesMonthEndValue()
        {
            // Arrange
            const double v0 = 0;
            const int nDaysJan = 31;
            var t1 = new DateTime(2015, 2, 23, 0, 0, 0);
            var totalDays = nDaysJan + t1.Day - 1;
            const double consumptionPerDay = 2;
            var v1 = v0 + consumptionPerDay * totalDays;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150101.Time, v0, Quality.Ok),
                new Tvq(t1, v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            const double expectedValue = consumptionPerDay * nDaysJan;
            Assert.Equal(expectedValue, x.V, 7);
        }

        [Fact]
        public void MonthlyAverage_When1ValueAtStartOfJanAnd1InFebAndFirstIs1000_InterpolatesJanEndValue()
        {
            // Arrange
            const double v0 = 1000;
            const double v1 = v0 + 600d;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150101.Time, v0, Quality.Ok),
                new Tvq(new DateTime(2015, 2, 23, 0, 0, 0), v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            const int nDaysJan = 31;
            const double totalDays = nDaysJan + 22;
            const double consumptionPerDay = (v1 - v0) / totalDays;
            const double expectedValue = consumptionPerDay * nDaysJan;
            Assert.Equal(expectedValue, x.V, 7);
        }

        [Fact]
        public void MonthlyAverage_When1ValueInMiddleOfJanAnd1InFeb_InterpolatesJanEndValue()
        {
            // Arrange
            const double v0 = 0;
            const double v1 = v0 + 700d;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150110.Time, v0, Quality.Ok),
                new Tvq(new DateTime(2015, 2, 23, 0, 0, 0), v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            var totalDays = (ts[1].Time - ts[0].Time).Days;
            var consumptionPerDay = (v1 - v0) / totalDays;
            var nDaysConsumptionJan = 1 + 31 - ts[0].Time.Day;
            var expectedValue = v0 + nDaysConsumptionJan * consumptionPerDay;

            Assert.Equal(expectedValue, x.V, 3);
        }

        
        [Fact]
        public void MonthlyAverage_When1ValueInMiddleOfJanAnd1InFeb_ExtrapolatesFebEndValue()
        {
            // Arrange
            const double v0 = 0;
            const double v1 = v0 + 700d;
            const double v2 = v1 + 200d;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150110.Time, v0, Quality.Ok),
                new Tvq(new DateTime(2015, 2, 13, 0, 0, 0), v1, Quality.Ok),
                new Tvq(new DateTime(2015, 2, 24, 0, 0, 0), v2, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            var totalDays = (ts[1].Time - ts[0].Time).Days;
            var consumptionPerDay = (v1 - v0) / totalDays;
            var nDaysConsumptionJan = 1 + 31 - ts[0].Time.Day;
            var expectedValue = v0 + nDaysConsumptionJan * consumptionPerDay;

            Assert.Equal(expectedValue, x.V, 3);
        }

        [Fact]
        public void MonthlyAverage_WhenConstantValueInJanAndFeb_ReturnsZero()
        {
            // Arrange
            // Act
            const double v = 1000;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150101.Time, v, Quality.Ok),
                new Tvq(m_Tvqs.Tvq20150110.Time, v, Quality.Ok),
                new Tvq(new DateTime(2015, 2, 21, 0, 0, 0), v, Quality.Ok)
            };
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            Assert.Equal(0, averages[0].V, 4);
        }

        [Fact]
        public void MonthlyAverage_WhenEndsInMonth_ExtrapolatesConsumptionToMonthEnd()
        {
            // Arrange
            const double v0 = 0;
            var t1 = new DateTime(2015, 1, 21, 0, 0, 0);
            var totalDays = t1.Day - 1;
            const double consumptionPerDay = 2;
            var v1 = v0 + consumptionPerDay * totalDays;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150101.Time, v0, Quality.Ok),
                new Tvq(t1, v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            const int nDaysJan = 31;
            const double expectedValue = consumptionPerDay * nDaysJan;
            Assert.Equal(expectedValue, x.V, 7);
        }

        [Fact]
        public void MonthlyAverage_WhenWholeMonthDiminishingConsumption_ConsumptionIsCorrect()
        {
            // Arrange
            const double v0 = 0;
            var t0 = new DateTime(2015, 09, 01, 0, 0, 0);
            var t1 = new DateTime(2015, 09, 16, 0, 0, 0);
            var t2 = new DateTime(2015, 10, 01, 0, 0, 0);
            double[] consumptionPerDay = {2d, 1d};
            
            var v1 = v0 + consumptionPerDay[0] * (t1 - t0).TotalDays;
            var v2 = v1 + consumptionPerDay[1] * (t2 - t1).TotalDays;
            var ts = new Timeseries
            {
                new Tvq(t0, v0, Quality.Ok),
                new Tvq(t1, v1, Quality.Ok),
                new Tvq(t2, v2, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            var expectedValue =
                (t1 - t0).TotalDays * consumptionPerDay[0] 
                + (t2 - t1).TotalDays * consumptionPerDay[1];
            Assert.Equal(expectedValue, x.V, 7);
        }

        [Fact]
        public void MonthlyAverage_WhenLastValueIsInNextMonth_ConsumptionIsCorrect()
        {
            // Arrange
            const double v0 = 1000;
            var t0 = new DateTime(2015, 09, 01, 0, 0, 0);
            var t1 = new DateTime(2015, 09, 11, 0, 0, 0);
            var t2 = new DateTime(2015, 10, 02, 0, 0, 0);
            double[] consumptionPerDay = {2d, 1d};
            
            var v1 = v0 + consumptionPerDay[0] * (t1 - t0).TotalDays;
            var v2 = v1 + consumptionPerDay[1] * (t2 - t1).TotalDays;
            var ts = new Timeseries
            {
                new Tvq(t0, v0, Quality.Ok),
                new Tvq(t1, v1, Quality.Ok),
                new Tvq(t2, v2, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var x = averages[0];
            var oct1 = new DateTime(2015, 10, 01, 0, 0, 0);
            var expectedValue0 =
                (t1 - t0).TotalDays * consumptionPerDay[0] 
                + (oct1 - t1).TotalDays * consumptionPerDay[1];
            Assert.Equal(expectedValue0, x.V, 7);

            var nov1 = oct1.AddMonths(1);
            var expectedValue1 = (nov1 - oct1).TotalDays * consumptionPerDay[1];
            Assert.Equal(expectedValue1, averages[1].V);
        }

        [Fact]
        public void MonthlyAverage_WhenConstantConsumptionInJanAndJune_FebThroughMayAreCorrect()
        {
            // Arrange
            const double s1 = 1000;
            const double k = 10d;
            var nDays = m_Tvqs.Tvq20150601.Time.DayOfYear - 1;
            var endValue = s1 + nDays * k;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150101.Time, s1, Quality.Ok),
                new Tvq(m_Tvqs.Tvq20150601.Time, endValue, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            Assert.Equal(31 * k, averages[0].V, 3);
            Assert.Equal(28 * k, averages[1].V, 3);
            Assert.Equal(31 * k, averages[2].V, 3);
            Assert.Equal(30 * k, averages[3].V, 3);
        }

        [Fact]
        public void MonthlyAverage_WhenFrom2015To2016_TimesAreCorrect()
        {
            // Arrange
            // Act
            const double v = 1000;
            var ts = new Timeseries
            {
                new Tvq(m_Tvqs.Tvq20150601.Time, v, Quality.Ok),
                new Tvq(m_Tvqs.Tvq20160601.Time, v, Quality.Ok)
            };
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            Assert.Equal(new DateTime(2015, 06, 01, 0, 0, 0), averages[0].Time);
            Assert.Equal(new DateTime(2015, 07, 01, 0, 0, 0), averages[1].Time);
            Assert.Equal(new DateTime(2015, 12, 01, 0, 0, 0), averages[6].Time);
            Assert.Equal(new DateTime(2016, 01, 01, 0, 0, 0), averages[7].Time);
            Assert.Equal(new DateTime(2016, 06, 01, 0, 0, 0), averages[12].Time);
        }

        [Fact]
        public void MonthlyAverage_WhenFrom2015To2017_June2016IsAverage()
        {
            // Arrange
            const int v0 = 300;
            const int v1 = 500;
            var t0 = m_Tvqs.Tvq20150601.Time;
            var t1 = new DateTime(2017, 07, 01, 0, 0, 0);
            var ts = new Timeseries
            {
                new Tvq(t0, v0, Quality.Ok),
                new Tvq(t1, v1, Quality.Ok)
            };

            // Act
            var averages = m_Periodizer.MonthlyAverage(ts);

            //Assert
            var tx = m_Tvqs.Tvq20160601.Time;
            var actual = averages.First(a => a.Time == tx);
            var consumptionPerDay = (v1 - v0) / (t1 - t0).TotalDays;
            var expected = consumptionPerDay * DateTime.DaysInMonth(tx.Year, tx.Month);
            Assert.Equal(expected, actual.V, 0);
        }

        [Fact]
        public void IsNewInterval_WhenMonthAndSameMonthDifferentYears_ReturnsTrue()
        {
            Assert.True(Periodizer.IsNewInterval(m_Tvqs.Tvq20150601.Time, m_Tvqs.Tvq20160601.Time, Interval.Month));
        }
    }
}
