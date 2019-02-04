using Xunit;
using Periodic;
using System.Linq;
using Periodic.Ts;

namespace PeriodicTest
{
    public class SplitterTest
    {
        Splitter m_Periodizer;
        Tvqs m_Tvqs;


        public SplitterTest()
        {
            m_Periodizer = new Splitter();
            m_Tvqs = new Tvqs();
        }

        [Fact]
        public void SplitPerYear_WhenEmpty_ReturnsEmptyEnumerable()
        {
            var result = m_Periodizer.SplitPerYear(new Timeseries());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SplitPerYear_WhenOnly20150101_ReturnsOneEnumerableWithTheValue()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Single(result);            
        }

        [Fact]
        public void SplitPerYear_WhenOneEachYear_ReturnsTwoEnumerablesWithEachOneValue()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101).Add(m_Tvqs.Tvq20160101);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Equal(2, result.Count);
            Assert.Single(result[0]);
            Assert.Single(result[1]);
            Assert.Equal(m_Tvqs.Tvq20150101, result[0].First());
            Assert.Equal(m_Tvqs.Tvq20160101, result[1].First());
        }

        [Fact]
        public void SplitPerYear_WhenOnlyOneYear_ReturnsOneEnumerableWithAllValues()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101).Add(m_Tvqs.Tvq20150601);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Single(result);
            Assert.Equal(2, result[0].Count);
            Assert.Equal(m_Tvqs.Tvq20150101, result[0][0]);
            Assert.Equal(m_Tvqs.Tvq20150601, result[0][1]);
        }

        [Fact]
        public void SplitPerYear_WhenTwoPerYear_Returns2EnumerablesWith2Values()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101)
                .Add(m_Tvqs.Tvq20150601)
                .Add(m_Tvqs.Tvq20160101)
                .Add(m_Tvqs.Tvq20160601);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[0].Count);
            Assert.Equal(2, result[1].Count);
            Assert.Equal(m_Tvqs.Tvq20150101, result[0][0]);
            Assert.Equal(m_Tvqs.Tvq20150601, result[0][1]);
            Assert.Equal(m_Tvqs.Tvq20160101, result[1][0]);
            Assert.Equal(m_Tvqs.Tvq20160601, result[1][1]);
        }

        [Fact]
        public void SplitPerWeek_WhenEmpty_ReturnsEmptyEnumerable()
        {
            var result = m_Periodizer.SplitPerWeek(new Timeseries());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void SplitPerWeek_WhenOnly20150101_ReturnsOneEnumerableWithTheValue()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101);

            var result = m_Periodizer.SplitPerWeek(source).ToList();

            Assert.Single(result);            
        }

        [Fact]
        public void SplitPerWeek_WhenOneEachWeek_ReturnsThreeEnumerablesWithEachOneValue()
        {
            var ts = new Timeseries();
            var source = ts
                .Add(m_Tvqs.Tvq20150101)
                .Add(m_Tvqs.Tvq20150110)
                .Add(m_Tvqs.Tvq20150120);

            var result = m_Periodizer.SplitPerWeek(source).ToList();

            Assert.Equal(3, result.Count);
            Assert.Single(result[0]);
            Assert.Single(result[1]);
            Assert.Single(result[2]);
            Assert.Equal(m_Tvqs.Tvq20150120, result[0].First());
            Assert.Equal(m_Tvqs.Tvq20150110, result[1].First());
            Assert.Equal(m_Tvqs.Tvq20150101, result[2].First());
        }

        [Fact]
        public void SplitPerWeek_WhenOnlyOneWeek_ReturnsOneEnumerableWithAllValues()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101).Add(m_Tvqs.Tvq20150105);

            var result = m_Periodizer.SplitPerWeek(source).ToList();

            Assert.Single(result);
            Assert.Equal(2, result[0].Count);
            Assert.Equal(m_Tvqs.Tvq20150101, result[0][0]);
            Assert.Equal(m_Tvqs.Tvq20150105, result[0][1]);
        }


    }
}
