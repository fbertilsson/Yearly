using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Periodic.Ts;

namespace Periodic
{
    /// <summary>
    /// Renders a list of timeseries containing one value per day
    /// as a text matrix.
    ///
    /// <example>
    /// <code>
    /// 		aktuell
    /// 20190101	0,00
    /// 20190102	1,00
    /// 20190103	4,00
    /// 20190104	9,00
    /// 20190105	16,00
    /// 20190106	25,00
    /// 20190107	36,00
    /// </code>
    /// </example>
    /// </summary>
    public class DailyValueTextRenderer
    {
        private readonly CultureInfo m_CultureInfo;
        public char ColumnSeparator { get; private set; }

        public DailyValueTextRenderer(CultureInfo cultureInfo, char columnSeparator = '\t')
        {
            m_CultureInfo = cultureInfo;
            ColumnSeparator = columnSeparator;
        }

        public void Render(IList<Timeseries> splitPerWeek, StringWriter writer)
        {
            WriteSeriesHeaders(splitPerWeek, writer);

            for (var i = 0; i < 7; i++)
            {
                //WriteRowHeader(i, writer);
                var isFirst = true;
                foreach (var series in splitPerWeek)
                {
                    var firstMonthInSeries = series[0].Time.Month; 
                    var startMonthDelta = firstMonthInSeries - 1;
                    var index = i - startMonthDelta;

                    if (index >= 0 && index < series.Count)
                    {
                        if (isFirst)
                        {
                            WriteRowHeader(series[i].Time, writer);
                            isFirst = false;
                        }

                        var v = series[index].V;
                        var numberFormat = m_CultureInfo.NumberFormat;
                        var formattedValue = v.ToString(numberFormat);
                        
                        writer.Write(ColumnSeparator);
                        writer.Write(formattedValue);
                    }
                }
                writer.WriteLine();
            }
        }
        private void WriteSeriesHeaders(IEnumerable<Timeseries> tsList, StringWriter writer)
        {
            int index = 0;
            foreach (var ts in tsList)
            {
                var header = index == 0 ? "aktuell" : $"v - {index}";
                writer.Write(ColumnSeparator);
                writer.Write(header);
                index++;
            }
            writer.WriteLine();
        }

        private void WriteRowHeader(DateTime t, StringWriter writer)
        {
            writer.Write(t.ToString("yyyyMMdd"));
        }

    }
}
