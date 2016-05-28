using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Periodic.Ts;

namespace Periodic
{
    /// <summary>
    /// Renders a list of timeseries containing one value per month
    /// as a text matrix.
    /// </summary>
    public class MonthlyValueTextRenderer
    {
        private readonly CultureInfo m_CultureInfo;
        private string ColumnSeparator { get; set; }

        public MonthlyValueTextRenderer(CultureInfo cultureInfo, string columnSeparator = "\t")
        {
            m_CultureInfo = cultureInfo;
            ColumnSeparator = columnSeparator;
        }

        public void Render(IList<Timeseries> splitPerYear, StringWriter writer)
        {
            WriteSeriesHeaders(splitPerYear, writer);

            var maxEntries = splitPerYear.Max(x => x.Count);
            for (var i = 0; i < maxEntries; i++)
            {
                var monthNumber = i + 1;
                WriteRowHeader(monthNumber, writer);

                foreach (var series in splitPerYear)
                {
                    var firstMonthInSeries = series[0].Time.Month;
                    var startMonthDelta = firstMonthInSeries - 1;
                    var index = i - startMonthDelta;

                    if (index >= 0 && index < series.Count)
                    {
                        var v = series[index].V;
                        var numberFormat = m_CultureInfo.NumberFormat;
                        var formattedValue = v.ToString(numberFormat);
                        writer.Write(formattedValue);
                    }
                    writer.Write(ColumnSeparator);
                }
                writer.WriteLine();
            }
        }
        private void WriteSeriesHeaders(IEnumerable<Timeseries> tsList, StringWriter writer)
        {
            writer.Write(ColumnSeparator);
            foreach (var ts in tsList)
            {
                writer.Write(ts.First().Time.Year);
                writer.Write(ColumnSeparator);
            }
            writer.WriteLine();
        }

        private void WriteRowHeader(int monthNumber, StringWriter writer)
        {
            if (UseAbbreviatedMonthName)
            {
                writer.Write(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(monthNumber));
            }
            else
            {
                writer.Write(monthNumber);
            }
            writer.Write(ColumnSeparator);
        }

        public bool UseAbbreviatedMonthName { get; set; }
    }
}
