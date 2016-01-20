using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataPage : Page
    {
        public DataPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartcontent();
        }

        private void LoadChartcontent()
        {
            List<TimeDistance> list = new List<TimeDistance>();
            foreach (var data in App.instance.transfer.data.currentwalkedRoute)
            {
                list.Add(new TimeDistance { Time = data.time.Hour + ":" + data.time.Minute + ":" + data.time.Second, Distance = data.distance });
            }
            (TimeChart.Series[0] as LineSeries).ItemsSource = list;
        }

        private void Ref_Click(object sender, RoutedEventArgs e)
        {
            LoadChartcontent();
        }
    }
    public class TimeDistance
    {
        public string Time { get; set; }
        public double Distance { get; set; }
    }
}
