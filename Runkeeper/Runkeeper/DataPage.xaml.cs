using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            double totalDistance = 0;
            ObservableCollection<TimeDistance> list = new ObservableCollection<TimeDistance>();
            for (int i = 0; i < App.instance.transfer.data.currentwalkedRoute.route.Count; i++)
            {
                if(App.instance.transfer.data.currentwalkedRoute.route.Count > 10)
                {
                    if (i % (App.instance.transfer.data.currentwalkedRoute.route.Count / 10) == 0)
                    {
                        totalDistance += App.instance.transfer.data.currentwalkedRoute.route[i].distance;
                        list.Add(new TimeDistance { Time = App.instance.transfer.data.currentwalkedRoute.route[i].time.Minute + ":" + App.instance.transfer.data.currentwalkedRoute.route[i].time.Second, Distance = totalDistance });
                    }
                }
                else
                {
                    totalDistance += App.instance.transfer.data.currentwalkedRoute.route[i].distance;
                    list.Add(new TimeDistance { Time = App.instance.transfer.data.currentwalkedRoute.route[i].time.Minute + ":" + App.instance.transfer.data.currentwalkedRoute.route[i].time.Second, Distance = totalDistance });
                }
                
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
