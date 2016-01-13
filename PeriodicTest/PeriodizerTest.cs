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

        public PeriodizerTest()
        {
            m_Tvqs = new Tvqs();
        }

        [Fact]
        public void InsertPoints_WhenNullThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Periodizer().InsertPoints(null, Interval.Year));
        }

        [Fact]
        public void InsertPoints_WhenEmptyTs_ReturnsEmptyTs()
        {
            Assert.Equal(0, new Periodizer().InsertPoints(new Timeseries(), Interval.Year).Count);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointAtLastSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150101};
            var result = new Periodizer().InsertPoints(ts, Interval.Year);
            var tvq = result.Last();
            Assert.Equal(new DateTime(2015, 12, 31, 23, 59, 59), tvq.Time);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointWithSameValueAsInterpolated()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20150101 };
            var result = new Periodizer().InsertPoints(ts, Interval.Year);
            var tvq = result.Last();
            Assert.Equal(ts[0].V, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenOnePoint_InsertsStartpointAtFirstSecond()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150601};
            var result = new Periodizer().InsertPoints(ts, Interval.Year);
            var tvq = result.First();
            Assert.Equal(new DateTime(2015, 01, 01, 0, 0, 0), tvq.Time);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtStart_InsertsEndpointWithSameValue()
        {
            var ts = new Timeseries {m_Tvqs.Tvq20150101};
            var result = new Periodizer().InsertPoints(ts, Interval.Year);
            Assert.Equal(2, result.Count);
            var tvq = result.Last();
            Assert.Equal(m_Tvqs.Tvq20150101.V, tvq.V);
            Assert.Equal(Quality.Interpolated, tvq.Q);
        }

        [Fact]
        public void InsertPoints_WhenOnePointAtEnd_DoesNotInsertEndpoint()
        {
            var ts = new Timeseries { m_Tvqs.Tvq20151231 };
            var result = new Periodizer().InsertPoints(ts, Interval.Year);
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
            var result = new Periodizer().InsertPoints(ts, Interval.Year);

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
            var result = new Periodizer().InsertPoints(ts, Interval.Year);

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
    }
}
