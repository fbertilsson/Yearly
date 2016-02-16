using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Periodic;

namespace Yearly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char ColumnSeparator = '\t';

        public MainWindow()
        {
            InitializeComponent();

        }

        private void InsertPointsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ts = TsParser.ParseTimeseries(tbSource.Text);
                var result = new Periodizer().InsertPoints(ts, Interval.Year);
                tbResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.ToString();
            }
        }

        private void SplitYearlyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ts = TsParser.ParseTimeseries(tbSource.Text);
                var tsWithInserts = new Periodizer().InsertPoints(ts, Interval.Year);
                var splitPerYear = new Splitter().SplitPerYear(tsWithInserts);

                string result = string.Empty;
                var maxEntries = splitPerYear.Max(x => x.Count);
                for (var i = 0; i < maxEntries; i++)
                {
                    string line = string.Empty;
                    foreach (var series in splitPerYear)
                    {
                        if (series.Count > i)
                        {
                            line += series[i];
                            line += ColumnSeparator;
                        }
                    }
                    result += line + "\r\n";
                }

                tbResult.Text = result;
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.ToString();
            }
        }

        private void SplitMonthlyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ts = TsParser.ParseTimeseries(tbSource.Text);
                var tsMonthly = new Periodizer().MonthlyAverage(ts);
                var splitPerYear = new Splitter().SplitPerYear(tsMonthly);

                var writer = new StringWriter();

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

                tbResult.Text = writer.ToString();
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.ToString();
            }
        }

        private void WriteSeriesHeaders(IList<Timeseries> tsList, StringWriter writer)
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

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            tbSource.Focus();
            tbSource.Text = @"2014-08-01 00:00	2
2014-09-26 00:07	3
2015-03-31 23:59	5
2015-06-01 00:00	2
2016-01-26 00:07	3
2016-10-31 23:59	5
";
            tbSource.SelectAll();
        }

    }
}
