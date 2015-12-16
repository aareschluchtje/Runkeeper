using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.System;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        public MapPage()
        {
            this.InitializeComponent();
            startTracking();
        }

        public async Task<Geoposition> GetPosition()
        {
            var accesStatus = await Geolocator.RequestAccessAsync();

            if (accesStatus != GeolocationAccessStatus.Allowed)
            {
                var succes = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
            }
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 , MovementThreshold = 1};
            geolocator.PositionChanged += Geolocator_PositionChanged;
            var position = await geolocator.GetGeopositionAsync();

            return position;
        }

        private async void startTracking()
        {
            Geoposition x = await GetPosition();
        }

        async private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentLocation(args.Position);
            });
        }

        private async void currentLocation(Geoposition position)
        {
            MapControl1.Center = position.Coordinate.Point;
            MapControl1.ZoomLevel = 16;
            MapIcon runner = new MapIcon();
            runner.Location = position.Coordinate.Point;
            runner.Title = "I am here";
            MapControl1.MapElements.Add(runner);
        }
    }
}
