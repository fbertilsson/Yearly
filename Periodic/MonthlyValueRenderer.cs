using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Periodic
{
    /// <summary>
    /// Renders a list of timeseries containing one value per month
    /// as a text matrix.
    /// </summary>
    public class MonthlyValueTextRenderer
    {
        private string ColumnSeparator { get; set; }

        public MonthlyValueTextRenderer(string columnSeparator = "\t")
        {
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
                        writer.Write(series[index].V);
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
            writer.Write(DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(monthNumber));
            writer.Write(ColumnSeparator);
        }
    }
}
