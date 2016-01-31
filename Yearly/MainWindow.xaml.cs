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
        }
    }
}
