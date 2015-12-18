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
using Windows.Services.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        private List<Geopoint> walkedRoute;
        private MapIcon runner;
        private MapPolyline line;

        public MapPage()
        {
            this.InitializeComponent();
         
        }
        
        public async Task<Geoposition> GetPosition()
        {
            var accesStatus = await Geolocator.RequestAccessAsync();

            if (accesStatus != GeolocationAccessStatus.Allowed)
            {
                var succes = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
            }
            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0, MovementThreshold = 1 };
            geolocator.PositionChanged += Geolocator_PositionChanged;
            var position = await geolocator.GetGeopositionAsync();

            return position;
        }

        private async void startTracking()
        {
            Geoposition x = await GetPosition();
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
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
            this.runner = new MapIcon();
            runner.Location = position.Coordinate.Point;
            runner.Title = "I am here";

            if(walkedRoute==null)
            {
                walkedRoute = new List<Geopoint>();
            }

            walkedRoute.Add(position.Coordinate.Point);

            UpdateWalkedRoute();

        }

        private async void UpdateWalkedRoute()
        {
            if (walkedRoute.Count >= 2)
            {
                MapRouteFinderResult e = await MapRouteFinder.GetWalkingRouteFromWaypointsAsync(walkedRoute);
                MapRoute b = e.Route;
                MapControl1.MapElements.Clear();
                MapControl1.MapElements.Add(runner);
                if(line != null)
                {
                    MapControl1.MapElements.Add(line);
                }
            }
        }

        public static async Task<MapLocation> FindLocation(string location, Geopoint reference)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(location, reference);
            MapLocation from = result.Locations.First();
            return from;
        }

        public static async Task<MapRoute> FindRunnerRoute(Geopoint from, Geopoint to)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(from,to);
            MapRoute b = routeResult.Route;
            return b;
        }

        private void StartRunning_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RunningPage));
        }

        private void Activiteiten_Click(object sender, RoutedEventArgs e)
        {
            startTracking();
        }

        private async void Route_Click(object sender, RoutedEventArgs e)
        {
            const string from = "Lovensdijkstraat, Breda";
            const string to = "nieuwe Inslag";

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(from, MapControl1.Center);
            MapLocation from1 = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(to, MapControl1.Center);

            MapLocation to1 = result.Locations.First();
            MapRouteFinderResult routeresult = await MapRouteFinder.GetWalkingRouteAsync(from1.Point,to1.Point);

            MapRoute map1 = routeresult.Route;

            var color = Colors.Red;
            this.line = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };
            line.Path = new Geopath(map1.Path.Positions);
        }
    }
}
