using System.Linq;
using Periodic.Algo;
using Periodic.Ts;
using PeriodicTest;
using Xunit;

namespace PeriodicTest2.Algo
{
    public class AggregateTest
    {
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private Aggregate _aggregate;
        private Average _average;

        private Tvqs _tvqs;

        private Timeseries _ts = new Timeseries();
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        public AggregateTest()
        {
            _aggregate = new Aggregate();
            _average = new Average();
            _tvqs = new Tvqs();
        }

        [Fact]
        public void Apply_WhenNull_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Assert.Empty(_aggregate.Apply(_average, null));
        }

        [Fact]
        public void Apply_WhenEmpty_ReturnsEmpty()
        {
            // Arrange
            // Act
            // Assert
            Assert.Empty(_aggregate.Apply(_average, _ts));
        }

        [Fact]
        public void Apply_WhenAverageAndOneValueAtStart_ReturnsValue()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150301);

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().V, _tvqs.Tvq20150301.V);
        }

        [Fact]
        public void Apply_WhenAverageAndOneValueAtStart_ReturnsMonthStart()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150301);

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().Time, _tvqs.Tvq20150301.Time);
        }

        [Fact]
        public void Apply_WhenAverageAndOneValueInMiddle_ReturnsValue()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150622);

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().V, _tvqs.Tvq20150622.V);
        }

        [Fact]
        public void Apply_WhenAverageAndOneValueInMiddle_ReturnsMonthStart()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150622);

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().Time, _tvqs.Tvq20150601.Time);
        }

        [Fact]
        public void Apply_WhenAverageAndTwoValuesInJune_ReturnsOneValueAtMonthStart()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150622);
            _ts.Add(_tvqs.Tvq20150630);

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().Time, _tvqs.Tvq20150601.Time);
            Assert.Single(result);
        }

        [Fact]
        public void Apply_WhenAverageAndTwoConstantValuesInJune_ReturnsTheValue()
        {
            // Arrange
            _ts.Add(_tvqs.Tvq20150601);
            var theConstantValue = _tvqs.Tvq20150601.V;
            _ts.Add(new Tvq(_tvqs.Tvq20150630.Time, theConstantValue, Quality.Ok));

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(result.First().V, theConstantValue);
        }

        [Fact]
        public void Apply_WhenAverageAndValueRisesFrom0To10_Returns5()
        {
            // Arrange
            _ts.Add(new Tvq(_tvqs.Tvq20150601.Time, 0, Quality.Ok));
            _ts.Add(new Tvq(_tvqs.Tvq20150630.Time, 10, Quality.Ok));

            // Act
            var result = _aggregate.Apply(_average, _ts);

            // Assert
            Assert.Equal(5d, result.First().V, 6);
        }

    }
}
