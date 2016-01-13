using Xunit;
using Periodic;
using System.Linq;

namespace PeriodicTest
{
    public class SplitterTest
    {
        Splitter m_Periodizer;
        Tvqs m_Tvqs;
        //Tvq m_Tvq20150101;
        //Tvq m_Tvq20150601;
        //Tvq m_Tvq20160101;
        //Tvq m_Tvq20160601;


        public SplitterTest()
        {
            m_Periodizer = new Splitter();
            m_Tvqs = new Tvqs();
            //var t = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            //m_Tvq20150101 = new Tvq(t, 100, Quality.Ok);
            //t = new DateTime(2015, 06, 01, 0, 0, 0, 0);
            //m_Tvq20150601 = new Tvq(t, 200, Quality.Ok);
            //t = new DateTime(2016, 01, 01, 0, 0, 0, 0);
            //m_Tvq20160101 = new Tvq(t, 300, Quality.Ok);
            //t = new DateTime(2016, 06, 01, 0, 0, 0, 0);
            //m_Tvq20160601 = new Tvq(t, 500, Quality.Ok);
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

            Assert.Equal(1, result.Count());            
        }

        [Fact]
        public void SplitPerYear_WhenOneEachYear_ReturnsTwoEnumerablesWithEachOneValue()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101).Add(m_Tvqs.Tvq20160101);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result[0].Count());
            Assert.Equal(1, result[1].Count());
            Assert.Equal(m_Tvqs.Tvq20150101, result[0].First());
            Assert.Equal(m_Tvqs.Tvq20160101, result[1].First());
        }

        [Fact]
        public void SplitPerYear_WhenOnlyOneYear_ReturnsOneEnumerableWithAllValues()
        {
            var ts = new Timeseries();
            var source = ts.Add(m_Tvqs.Tvq20150101).Add(m_Tvqs.Tvq20150601);

            var result = m_Periodizer.SplitPerYear(source).ToList();

            Assert.Equal(1, result.Count());
            Assert.Equal(2, result[0].Count());
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

            Assert.Equal(2, result.Count());
            Assert.Equal(2, result[0].Count());
            Assert.Equal(2, result[1].Count());
            Assert.Equal(m_Tvqs.Tvq20150101, result[0][0]);
            Assert.Equal(m_Tvqs.Tvq20150601, result[0][1]);
            Assert.Equal(m_Tvqs.Tvq20160101, result[1][0]);
            Assert.Equal(m_Tvqs.Tvq20160601, result[1][1]);
        }
    }
}
