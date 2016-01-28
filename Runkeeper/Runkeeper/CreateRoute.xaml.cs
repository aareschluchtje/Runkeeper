using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class CreateRoute : Page
    {
        public CreateRoute()
        {
            this.InitializeComponent();
        }

        private void StartOwnRoute_Click(object sender, RoutedEventArgs e)
        { 
            Frame.Navigate(typeof(MapPage),new Tuple<string,string,string>("createroute",From.Text,To.Text));
       
        }

        private void From_OnGotFocus(object sender, RoutedEventArgs e)
        {
            From.Text = "";
            Frame.Navigate(typeof (FromRoutePage),new Tuple<string>(From.Text));
        }

        private void To_OnGotFocus(object sender, RoutedEventArgs e)
        {
            To.Text = "";
   
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var value = (Tuple<string,string>)e.Parameter;
            if (value.Item1 != null)
            {
                From.Text = value.ToString();
            }
        }
    }
}
