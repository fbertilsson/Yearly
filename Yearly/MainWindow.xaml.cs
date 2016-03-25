using System;
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
    public partial class MainWindow
    {
        private const string ColumnSeparator = "\t";

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

                var renderer = new MonthlyValueTextRenderer(CultureInfo.CurrentCulture, ColumnSeparator);
                renderer.Render(splitPerYear, writer);

                tbResult.Text = writer.ToString();
            }
            catch (Exception ex)
            {
                tbResult.Text = ex.ToString();
            }
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
