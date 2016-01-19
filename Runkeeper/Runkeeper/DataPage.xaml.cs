using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            foreach (List<DataStamp> data in DataHandler.walkedRoutes)
            {
                foreach(DataStamp datastamp in data)
                {
                    list.Add(new TimeDistance { time = datastamp.time.ToString() } );
                }
            }
            (TimeChart.Series[0] as LineSeries).ItemsSource = list;
        } 

        private void Ref_Click(object sender, RoutedEventArgs e)
        {
            LoadChartcontent();
        }

        public double calculateDistance(Geopoint start, Geopoint end)
        {
            double distance = 0;
            double theta = start.Position.Longitude - end.Position.Longitude;
            double dist = Math.Sin(Math.PI*start.Position.Latitude/180)*Math.Sin(Math.PI*end.Position.Latitude/180) +
                          Math.Cos(Math.PI*start.Position.Latitude/180)*Math.Cos(Math.PI*end.Position.Latitude/180)*
                          Math.Cos(Math.PI*theta/180);
            dist = Math.Acos(dist);
            dist = dist/Math.PI*180;
            dist = dist*60*1.1515;
            distance = dist*1.609344;
            return distance;
        }
    }
    public class TimeDistance
    {
        public string time;
        public double distance;
    }
}
