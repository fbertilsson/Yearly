using Xunit;
using Periodic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void InsertPoints_WhenTwoPointsSameYear_InsertsExtrapolatedEndValue()
        {
            // Arrange
            var ts = new Timeseries {m_Tvqs.Tvq20150101, m_Tvqs.Tvq20150601};

            // Act
            var result = m_Periodizer.InsertPoints(ts, Interval.Year);

            // Assert
            Assert.Equal(3, result.Count);
            var tvq0 = result[0];
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
        public void MonthlyAverage_WhenOneValueFirstSecondInJan_ReturnsItAsAverage()
        {
            // Arrange
            // Act
            var averages = m_Periodizer.MonthlyAverage(new Timeseries {m_Tvqs.Tvq20150101});

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            Assert.Equal(m_Tvqs.Tvq20150101.V, x.V);
        }

        [Fact]
        public void MonthlyAverage_WhenOneValueInJan_ReturnsItAsAverageAtFirstSecond()
        {
            // Arrange
            // Act
            var averages = m_Periodizer.MonthlyAverage(new Timeseries { m_Tvqs.Tvq20150105 });

            //Assert
            var x = averages[0];
            Assert.Equal(m_Tvqs.Tvq20150101.Time, x.Time);
            const int nDays = 31 - 5 + 1;
            var expectedValue = m_Tvqs.Tvq20150105.V * nDays / 31;
            Assert.Equal(expectedValue, x.V, 5);
        }
    }
}
