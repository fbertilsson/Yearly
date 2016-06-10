using System;
using System.Collections.Generic;
using Xunit;
using System.Globalization;
using System.IO;
using System.Linq;
using Periodic;
using Periodic.Ts;

namespace PeriodicTest
{
    public class MonthlyValueTextRendererTest
    {
        private MonthlyValueTextRenderer m_Renderer;
        private StringWriter m_Writer;
        private Tvqs m_Tvqs;
        private Timeseries m_FebToDec;

        public MonthlyValueTextRendererTest()
        {
            m_Renderer = new MonthlyValueTextRenderer(new CultureInfo("SV"));
            m_Writer = new StringWriter();
            m_Tvqs = new Tvqs();
            m_FebToDec = Create(m_Tvqs.Tvq20160201.Time, 11, 200, 100);
        }

        private Timeseries Create(DateTime tvq20160201, int nValues, int v0, int delta)
        {
            var ts = new Timeseries();
            for (int i = 0; i < nValues; i++)
            {
                ts.Add(new Tvq(tvq20160201.AddMonths(i), v0 + i*delta, Quality.Ok));
            }
            return ts;
        }

        [Fact]
        public void Render_WhenOneValueInJanuary_RendersTheValue()
        {
            // Arrange
            var ts = new Timeseries {m_Tvqs.Tvq20160101};

            // Act
            m_Renderer.Render(new [] { ts }, m_Writer);

            // Assert
            var tokensWithoutNewlines = GetTokensAfterHeader(m_Writer);

            Assert.Equal("1", tokensWithoutNewlines[0]);
            Assert.Equal(m_Tvqs.Tvq20160101.V.ToString(CultureInfo.InvariantCulture), tokensWithoutNewlines[1]);
        }

        private List<string> GetTokensAfterHeader(StringWriter writer)
        {
            var tokens = writer.ToString().Split('\t');
            var tokensAfterHeader = tokens.Skip(2);
            var tokensWithoutNewlines = tokensAfterHeader.Select(x => x.Trim()).ToList();
            return tokensWithoutNewlines;
        }

        [Fact]
        public void Render_WhenFirstValueIsInFebruary_RendersJanuarysValueIsEmpty()
        {
            // Arrange
            var ts = new Timeseries { m_Tvqs.Tvq20160201 };

            // Act
            m_Renderer.Render(new[] { m_FebToDec }, m_Writer);

            // Assert
            var tokensWithoutNewlines = GetTokensAfterHeader(m_Writer);
            Assert.Equal("1", tokensWithoutNewlines[0]);
            Assert.Equal(String.Empty, tokensWithoutNewlines[1]);
        }

        [Fact]
        public void Render_WhenFirstValueIsInFebruary_RendersDecember()
        {
            // Arrange
            // Act
            m_Renderer.Render(new[] { m_FebToDec }, m_Writer);

            // Assert
            var tokensWithoutNewlines = GetTokensAfterHeader(m_Writer);
            var decemberRowHeaderIndex = 11*2;
            Assert.Equal("12", tokensWithoutNewlines[decemberRowHeaderIndex]);
            Assert.Equal(
                m_FebToDec.Last().V.ToString(CultureInfo.InvariantCulture), 
                tokensWithoutNewlines[1 + decemberRowHeaderIndex]);
        }
    }
}