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
        private Geopoint startposition;
        public MapControl mapcontrol;
        public string from, to;
        private List<Geopoint> waypoints;
  
        public MapPage()
        {
            this.InitializeComponent();
            this.waypoints = new List<Geopoint>();
            this.mapcontrol = MapControl1;
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
            MapControl1.ZoomLevel = 16;
            Geoposition x = await GetPosition();
            startposition = x.Coordinate.Point;
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
            this.runner = new MapIcon();
            runner.Location = position.Coordinate.Point;
            runner.Title = "I am here";
<<<<<<< HEAD
            runner.ZIndex = 3;
=======
            waypoints.Add(runner.Location);

>>>>>>> origin/master
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

                MapPolyline currentline = new MapPolyline
                {
                    StrokeThickness = 11,
                    StrokeColor = Colors.Blue,
                    StrokeDashed = false,
                    ZIndex = 2
                };
                List<BasicGeoposition> positions = new List<BasicGeoposition>();
                for (int i = 0; i < waypoints.Count; i++)
                {
                    positions.Add(new BasicGeoposition() { Latitude = waypoints[i].Position.Latitude, Longitude = waypoints[i].Position.Longitude });
                }
                currentline.Path = new Geopath(positions);
                MapControl1.MapElements.Clear();
                if(line != null)
                {
                    MapControl1.MapElements.Add(line);
                }
                MapControl1.MapElements.Add(currentline);
                MapControl1.MapElements.Add(runner);
            }
        }

        public async void FromToRoute(string from, string to)
        {
            this.from = from;
            this.to = to;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(from, MapControl1.Center);
            MapLocation from1 = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(to, MapControl1.Center);

            MapLocation to1 = result.Locations.First();
            MapRouteFinderResult routeresult = await MapRouteFinder.GetWalkingRouteAsync(from1.Point, to1.Point);

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
            
        }

        private async void Route_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateRoute), new Tuple<string, string>(from,to));
        }

        private void GetOwnLocation_Click(object sender, RoutedEventArgs e)
        {
            startTracking();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var value = (Tuple<string, string,string>)e.Parameter;
            if(value.Item1.Equals("createroute"))
            {
                FromToRoute(value.Item2, value.Item3);
            }
        }
    }
}
