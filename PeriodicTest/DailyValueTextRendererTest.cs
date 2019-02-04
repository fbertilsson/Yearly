using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Periodic;
using Periodic.Ts;
using PeriodicTest;
using Xunit;

namespace PeriodicTest2
{
    public class DailyValueTextRendererTest
    {
        private DailyValueTextRenderer _renderer;
        private Tvqs _tvqs;

        public DailyValueTextRendererTest()
        {
            _renderer = new DailyValueTextRenderer(CultureInfo.GetCultureInfo("sv-SE"));
            _tvqs = new Tvqs();
        }

        [Fact]
        public void Render_WhenOneValue_RendersColumnHeaders()
        {
            var list = new List<Timeseries>
            {
                new Timeseries().Add(_tvqs.Tvq20150101)
            };
            
            var writer = new StringWriter();
            _renderer.Render(list, writer);

            // Assert
            var headers = writer.ToString().Split(_renderer.ColumnSeparator);
            Assert.Equal(string.Empty, headers[0]);
            Assert.Equal("aktuell", headers[1]);
        }

        [Fact]
        public void Render_WhenOneValue_RendersRowHeaders()
        {
            var list = new List<Timeseries>
            {
                new Timeseries().Add(_tvqs.Tvq20150101)
            };

            var writer = new StringWriter();
            _renderer.Render(list, writer);

            var lines = writer.ToString().Replace(Environment.NewLine, "\n").Split('\n');
            Assert.Equal(9, lines.Length);

            Assert.Equal($"tor{_renderer.ColumnSeparator}100{_renderer.ColumnSeparator}", lines[1]);
            Assert.Equal($"1{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[2]);
            Assert.Equal($"2{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[3]);
            Assert.Equal($"3{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[4]);
            Assert.Equal($"4{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[5]);
            Assert.Equal($"5{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[6]);
            Assert.Equal($"6{_renderer.ColumnSeparator}{_renderer.ColumnSeparator}", lines[7]);
        }

        [Fact]
        public void Render_When7Values_RendersOneColumnWithCorrectValues()
        {
            var ts = new Timeseries();
            var t = new DateTime(2019, 01, 01, 0, 0, 0, 0);
            for (var i = 0; i < 7; i++)
            {
                ts.Add(new Tvq(t.AddDays(i), i * i, Quality.Ok));
            }
            var list = new List<Timeseries> { ts };

            var writer = new StringWriter();
            _renderer.Render(list, writer);

            var lines = writer.ToString().Replace(Environment.NewLine, "\n").Split('\n');
            Assert.Equal(9, lines.Length);

            Assert.Equal("0", GetColumn(1, lines[1]));
            Assert.Equal("1", GetColumn(1, lines[2]));
            Assert.Equal("4", GetColumn(1, lines[3]));
            Assert.Equal("9", GetColumn(1, lines[4]));
            Assert.Equal("16", GetColumn(1, lines[5]));
            Assert.Equal("25", GetColumn(1, lines[6]));
            Assert.Equal("36", GetColumn(1, lines[7]));
        }

        private string GetColumn(int columnIndex, string s)
        {
            return s.Split(_renderer.ColumnSeparator)[columnIndex];
        }
    }
}
