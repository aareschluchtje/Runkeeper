using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
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
    public sealed partial class FromRoutePage : Page
    {
        public string HuidigePlaats;
        public FromRoutePage()
        {
            this.InitializeComponent();
        }

        private void From_OnGotFocus(object sender, RoutedEventArgs e)
        {
            From.Text = "";
        }

        private async void Location_OnClick(object sender, RoutedEventArgs e)
        {
            Geopoint x = App.instance.transfer.data.currentposition.Location;
            await FindAddress(x);
            Frame.Navigate(typeof(CreateRoute), new Tuple<string, string>(From.Text, App.instance.transfer.data.to));
        }

        public async Task<String> FindAddress(Geopoint p)
        {
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAtAsync(p);

            string returnstring = "";

            if (result.Status == MapLocationFinderStatus.Success)
            {
                MapAddress address = result.Locations[0].Address;
                returnstring += (address.BuildingName == "" ? "" : address.BuildingName + ", ");
                returnstring += (address.Street == "" ? "" : address.Street + (address.StreetNumber == "" ? ", " : " " + address.StreetNumber + ", "));
                returnstring += address.Town;
            }
            From.Text = returnstring;
            return returnstring;
        }

        public async Task<String> FindAddress(double latitude, double longitude)
        {
            Geopoint p = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude });
            string address = await FindAddress(p);
            return address;
        }
    }
}
