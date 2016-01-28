using Runkeeper.ViewModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RunningPage : Page
    {
        
        public RunningPage()
        {
            this.InitializeComponent();
            SpeedText.DataContext = App.instance.transfer.data;
            TimeBlock.DataContext = App.instance.transfer.data.time;
            Afstand.DataContext = App.instance.transfer.data;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            App.instance.transfer.data.time.Stop();
            App.instance.transfer.data.saveData();
            MapPage.instance.StopLocating();
            TimeBlock.Text = "0:0:0:0";
            speedblock.Text = "0";
            afstandtext.Text = "0";
        }

        private async void START_Click(object sender, RoutedEventArgs e)
        {
            App.instance.transfer.data.time.Start();
            Geoposition x = await MapPage.instance.startLocating();
        }
    }
}
