using System;
using Periodic.Algo;
using Periodic.Ts;
using Xunit;

namespace PeriodicTest.Algo
{
    public class DeltaTvqOperatorTest
    {
        private DeltaTvqOperator m_Delta;
        private DateTime m_t0;
        private DateTime m_t1;
        private Tvq m_Tvq0;
        private Tvq m_Tvq5;
        private Tvq m_Tvq7;
        private Tvq m_Tvq11;
        private Tvq m_Tvq6k;
        private Tvq m_Tvq999991;
        private Tvq m_Tvq999981;


        public DeltaTvqOperatorTest()
        {
            m_Delta = new DeltaTvqOperator();

            m_t0 = new DateTime(2015, 01, 01, 0, 0, 0, 0);
            m_t1 = new DateTime(2015, 02, 01, 0, 0, 0, 0);

            m_Tvq0 = new Tvq(m_t0, 0, Quality.Ok);
            m_Tvq6k = new Tvq(m_t0, 611000, Quality.Ok);
            m_Tvq999991 = new Tvq(m_t0, 999991, Quality.Ok);
            m_Tvq5 = new Tvq(m_t0, 5, Quality.Ok);
            m_Tvq7 = new Tvq(m_t1, 7, Quality.Ok);
            m_Tvq11 = new Tvq(m_t1, 11, Quality.Ok);
            m_Tvq999981 = new Tvq(m_t1, 999981, Quality.Ok);
        }

        [Fact]
        public void Apply_When5And7_DeltaIs2()
        {
            var result = m_Delta.Apply(m_Tvq5, m_Tvq7);
            Assert.Equal(2, result.V);
        }

        [Fact]
        public void Apply_When5And11_DeltaIs6()
        {
            var result = m_Delta.Apply(m_Tvq5, m_Tvq11);
            Assert.Equal(6, result.V);
        }

        [Fact]
        public void Apply_When11And5_DeltaIsMinus6()
        {
            var result = m_Delta.Apply(m_Tvq11, m_Tvq5);
            Assert.Equal(-6, result.V);
        }

        [Fact]
        public void Apply_When5And11_TimeIsT1()
        {
            var result = m_Delta.Apply(m_Tvq5, m_Tvq11);
            Assert.Equal(m_t1, result.Time);
        }

        [Fact]
        public void Apply_WhenV2IsCloseToZero_AssumeNewMeterAndZeroConsumption()
        {
            var result = m_Delta.Apply(m_Tvq6k, m_Tvq11);
            Assert.Equal(0, result.V);
        }

        [Fact]
        public void Apply_When999991And000011_DeltaIs20()
        {
            var result = m_Delta.Apply(m_Tvq999991, m_Tvq11);
            Assert.Equal(20, result.V);
        }

        [Fact]
        public void Apply_When999991And999981_DeltaIsMinus10()
        {
            var result = m_Delta.Apply(m_Tvq999991, m_Tvq999981);
            Assert.Equal(-10, result.V);
        }

        [Fact]
        public void Apply_When6kAnd0_DeltaIs0()
        {
            var result = m_Delta.Apply(m_Tvq6k, m_Tvq0);
            Assert.Equal(0, result.V);
        }

        [Fact]
        public void Apply_When1kAnd0_DeltaIs0()
        {
            var result = m_Delta.Apply(new Tvq(m_t0, 1e3, Quality.Ok), m_Tvq0);
            Assert.Equal(0, result.V);
        }
        
        [Fact]
        public void Apply_WhenBothOk_ResultIsOk()
        {
            var result = m_Delta.Apply(m_Tvq11, m_Tvq5);
            Assert.Equal(Quality.Ok, result.Q);
        }

        [Fact]
        public void Apply_WhenFirstIsSuspect_ResultIsSuspect()
        {
            var suspect = new Tvq(m_t0, 0, Quality.Suspect);
            var result = m_Delta.Apply(suspect, m_Tvq5);
            Assert.Equal(Quality.Suspect, result.Q);
        }

        [Fact]
        public void Apply_WhenSecondIsSuspect_ResultIsSuspect()
        {
            var suspect = new Tvq(m_t0, 0, Quality.Suspect);
            var result = m_Delta.Apply(m_Tvq5, suspect);
            Assert.Equal(Quality.Suspect, result.Q);
        }

        [Fact]
        public void Apply_WhenSecondIsInterpolated_ResultIsInterpolated()
        {
            var interpolated = new Tvq(m_t0, 0, Quality.Interpolated);
            var result = m_Delta.Apply(m_Tvq5, interpolated);
            Assert.Equal(Quality.Interpolated, result.Q);
        }

    }
}
