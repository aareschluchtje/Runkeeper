using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Runkeeper.Model;
using System.ComponentModel;
using Windows.Devices.Geolocation.Geofencing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MapPage()
        {
            this.InitializeComponent();
            App.instance.transfer.data.currentwalkedRoute = new ObservableCollection<DataStamp>();
            startTracking();
        }
        
        public async Task<Geoposition> GetPosition()
        {
            var accesStatus = await Geolocator.RequestAccessAsync();

            if (accesStatus != GeolocationAccessStatus.Allowed)
            {
                var succes = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
            }
            var geofences = GeofenceMonitor.Current.Geofences;

            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged += Current_StatusChanged;

            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0, MovementThreshold = 1 };
            geolocator.PositionChanged += Geolocator_PositionChanged;
            var position = await geolocator.GetGeopositionAsync();
            return position;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            GeofenceMonitor.Current.GeofenceStateChanged -= Current_GeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged -= Current_StatusChanged;

            base.OnNavigatingFrom(e);
        }

        private void Current_StatusChanged(GeofenceMonitor sender, object args)
        {
            throw new NotImplementedException();
        }

        private async void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch(sender.Status)
                {
                    case GeofenceMonitorStatus.Ready:
                        //MainPage.NotifyUser("The monitor is ready and active.", NotifyType.StatusMessage);
                        break;
                }
            });
        }

        private void createGeofence()
        {
            // Set the fence ID.
            string fenceId = "fence1";

            BasicGeoposition position = new BasicGeoposition{ Latitude = App.instance.transfer.data.currentposition.Location.Position.Latitude, Longitude = App.instance.transfer.data.currentposition.Location.Position.Longitude};
            // Define the fence location and radius.
            double radius = 10; // in meters

            // Set a circular region for the geofence.
            Geocircle geocircle = new Geocircle(position, radius);

            // Create the geofence.
            Geofence geofence = new Geofence(fenceId, geocircle);
        }

        private async void startTracking()
        {
            MapControl1.ZoomLevel = 16;
            Geoposition x = await GetPosition();
            App.instance.transfer.data.startposition = x.Coordinate.Point;
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
            App.instance.transfer.data.currentposition = new MapIcon();
            App.instance.transfer.data.currentposition.Location = position.Coordinate.Point;
            double speed = Double.Parse(App.instance.transfer.data.speedChanges(position.Coordinate.Speed.ToString()));
            NotifyPropertyChanged(nameof(App.instance.transfer.data.currentSpeed));

            App.instance.transfer.data.currentposition.Title = "I am here";

            App.instance.transfer.data.currentposition.ZIndex = 3;

            if (App.instance.transfer.data.currentwalkedRoute.Count != 0)
            {
                double distance = await App.instance.transfer.data.calculateUpdateDistance(App.instance.transfer.data.currentwalkedRoute[App.instance.transfer.data.currentwalkedRoute.Count - 1].location, position.Coordinate.Point);
                App.instance.transfer.data.currentwalkedRoute.Add(new DataStamp(position.Coordinate.Point, DateTime.Now, speed, distance));
            }
            else
            {
                App.instance.transfer.data.currentwalkedRoute.Add(new DataStamp(position.Coordinate.Point, DateTime.Now, 0, 0));
            }

            UpdateWalkedRoute();
        }

        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private void UpdateWalkedRoute()
        {
            if (App.instance.transfer.data.currentwalkedRoute.Count >= 2)
            {
                MapPolyline currentline = new MapPolyline
                {
                    StrokeThickness = 11,
                    StrokeColor = Colors.Blue,
                    StrokeDashed = false,
                    ZIndex = 2
                };
                MapPolyline oldline = new MapPolyline
                {
                    StrokeThickness = 11,
                    StrokeColor = Colors.Gray,
                    StrokeDashed = false,
                    ZIndex = 1
                };
                List<BasicGeoposition> positions = new List<BasicGeoposition>();
                for (int i = 0; i < App.instance.transfer.data.currentwalkedRoute.Count; i++)
                {
                    positions.Add(new BasicGeoposition() { Latitude = App.instance.transfer.data.currentwalkedRoute[i].location.Position.Latitude, Longitude = App.instance.transfer.data.currentwalkedRoute[i].location.Position.Longitude });
                }
                List<BasicGeoposition> oldpositions = new List<BasicGeoposition>();
                foreach (ObservableCollection<DataStamp> route in App.instance.transfer.data.walkedRoutes)
                {
                    foreach (DataStamp point in route)
                    {
                        oldpositions.Add(new BasicGeoposition() { Latitude = point.location.Position.Latitude, Longitude = point.location.Position.Longitude });
                    }
                }
                currentline.Path = new Geopath(positions);
                if(oldpositions.Count != 0)
                {
                    oldline.Path = new Geopath(oldpositions);
                }
                
                MapControl1.MapElements.Clear();
                if(App.instance.transfer.data.calculatedRoute != null)
                {
                    MapControl1.MapElements.Add(App.instance.transfer.data.calculatedRoute);
                }
                MapControl1.MapElements.Add(currentline);

                if(oldline.Path != null)
                {
                    MapControl1.MapElements.Add(oldline);
                }
            }
            if(App.instance.transfer.data.currentposition != null)
            MapControl1.MapElements.Add(App.instance.transfer.data.currentposition);
        }

        public async void FromToRoute(string from, string to)
        {
            App.instance.transfer.data.from = from;
            App.instance.transfer.data.to = to;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(from, MapControl1.Center);
            MapLocation from1 = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(to, MapControl1.Center);

            MapLocation to1 = result.Locations.First();
            MapRouteFinderResult routeresult = await MapRouteFinder.GetWalkingRouteAsync(from1.Point, to1.Point);

            MapRoute map1 = routeresult.Route;

            var color = Colors.Red;
            App.instance.transfer.data.calculatedRoute = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };
            App.instance.transfer.data.calculatedRoute.Path = new Geopath(map1.Path.Positions);
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

        private void Route_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateRoute), new Tuple<string, string>(App.instance.transfer.data.from, App.instance.transfer.data.to));
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
