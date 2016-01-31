using System;
using System.Windows;
using Periodic;

namespace Yearly
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void ButtonClick(object sender, RoutedEventArgs e)
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
