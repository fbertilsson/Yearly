using System;
using Periodic.Algo;
using Periodic.Ts;
using Xunit;

namespace PeriodicTest.Algo
{
    public class AverageTest
    {
        Tvqs m_Tvqs;

        public AverageTest()
        {
            m_Tvqs = new Tvqs();
        }

        [Fact]
        public void Apply_WhenFirstValueIsInsidePeriod_AssumesStepwise()
        {
            // Arrange
            var tvq1 = new Tvq(
                new DateTime(2015, 01, 10, 0, 0, 0, 0),
                110,
                Quality.Ok);
            var tvq2 = new Tvq(
                new DateTime(2015, 01, 20, 0, 0, 0, 0),
                120,
                Quality.Ok);
            var ts = new Timeseries { tvq1, tvq2 };

            // Act
            var t0 = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            var t1 = new DateTime(2015, 01, 31, 23, 59, 59, 999);
            var result = new Average().Apply(t0, t1, ts);

            // Assert
            var area1 = tvq1.V*(tvq1.Time - t0).TotalSeconds;
            var area2 = (tvq2.Time - tvq1.Time).TotalSeconds*(tvq1.V + tvq2.V)/2;
            var area3 = (t1 - tvq2.Time).TotalSeconds*tvq2.V;
            var expected = (area1 + area2 + area3) / (t1 - t0).TotalSeconds;
            Assert.Equal(expected, result.V);
        }

        //[Fact]
        //public void Apply_When10thIs110And20thIs120_ExtrapolatesValueAt1stTo100()
        //{
        //    // Arrange
        //    var ts = new Timeseries
        //    {
        //        new Tvq(
        //            new DateTime(2015, 01, 10, 0, 0, 0, 0),
        //            110,
        //            Quality.Ok),
        //        new Tvq(
        //            new DateTime(2015, 01, 20, 0, 0, 0, 0),
        //            120,
        //            Quality.Ok)
        //    };

        //    // Act
        //    var t0 = new DateTime(2015, 01, 01, 0, 0, 0, 0);
        //    var t1 = new DateTime(2015, 01, 31, 23, 59, 59, 999);
        //    var result = new Average().Apply(t0, t1, ts);

        //    // Assert
        //    var expected = (100 + 131) / 2;
        //    Assert.Equal(expected, result.V);
        //}
    }
}
