using System;
using Xunit;
using Periodic.Algo;
using Periodic.Ts;

namespace PeriodicTest.Algo
{
    public class DeltaTsOperatorTest
    {
        private DeltaTsOperator m_Delta;
        private DateTime m_t0;
        private DateTime m_t1;
        private DateTime m_t2;
        private Tvq m_Tvq5;
        private Tvq m_Tvq7;
        private Tvq m_Tvq11;

        public DeltaTsOperatorTest()
        {
            m_Delta = new DeltaTsOperator();
            m_t0 = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            m_t1 = new DateTime(2015, 02, 01, 0, 0, 0, 0);
            m_t2 = new DateTime(2015, 02, 02, 0, 0, 0, 0);

            m_Tvq5 = new Tvq(m_t0, 5, Quality.Ok);
            m_Tvq7 = new Tvq(m_t1, 7, Quality.Ok);
            m_Tvq11 = new Tvq(m_t2, 11, Quality.Ok);
        }

        [Fact]
        public void Apply_WhenTsIsNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => m_Delta.Apply(null));
        }

        [Fact]
        public void Apply_WhenTsHasOneValue_ThrowsArgumentException()
        {
            var ts = new Timeseries {new Tvq(m_t0, 0, Quality.Ok)};
            Assert.Throws<ArgumentException>(() => m_Delta.Apply(ts));
        }

        [Fact]
        public void Apply_When5And7_DeltaIs2()
        {
            var ts = new Timeseries { m_Tvq5, m_Tvq7, };
            var result = m_Delta.Apply(ts);
            Assert.Equal(2, result[0].V);
        }


        [Fact]
        public void Apply_WhenThreeValues_ResultHasTwoCurrectValues()
        {
            var ts = new Timeseries { m_Tvq5, m_Tvq7, m_Tvq11, };
            var result = m_Delta.Apply(ts);
            Assert.Equal(2, result.Count);
            Assert.Equal(m_t1, result[0].Time);
            Assert.Equal(m_t2, result[1].Time);
            Assert.Equal(2, result[0].V);
            Assert.Equal(4, result[1].V);
            Assert.Equal(Quality.Ok, result[0].Q);
            Assert.Equal(Quality.Ok, result[1].Q);
        }
    }
}
