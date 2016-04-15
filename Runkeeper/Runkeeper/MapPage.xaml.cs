using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Runkeeper.Model;
using Windows.Devices.Geolocation.Geofencing;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Runkeeper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        private MapHelper maphelper = new MapHelper();
        private MapPolyline oldline;
        private Geolocator geolocator;
        public static MapPage instance;

        public MapPage()
        {
            instance = this;
            this.InitializeComponent();
            if(App.instance.transfer.data.currentposition != null && App.instance.transfer.data.currentwalkedRoute != null)
            {
                MapControl1.Center = App.instance.transfer.data.currentposition.Location;
                MapControl1.ZoomLevel = 16;
                UpdateWalkedRoute(App.instance.transfer.data.currentposition.Location);
            }
            if(!App.instance.transfer.data.startApp)
            {
                startTracking();
            }
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        public async Task<Geoposition> GetPosition()
        {
            var accesStatus = await Geolocator.RequestAccessAsync();

            if (accesStatus != GeolocationAccessStatus.Allowed)
            {
                var succes = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
            }

            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            //GeofenceMonitor.Current.StatusChanged += Current_StatusChanged;
  
            foreach (Route route in App.instance.transfer.data.walkedRoutes)
            {
                for (int i = 0; i < route.route.Count; i++)
                {
                    Geofence geofence = GeofenceHandler.createGeofence(route.route[i].location);
                    List<Geofence> geofences = GeofenceMonitor.Current.Geofences.ToList();
                    bool equal = false;
                    foreach (Geofence something in geofences)
                    {
                        if (something.Id == geofence.Id)
                        {
                            equal = true;
                        }
                    }
                    if(!equal)
                    {
                        GeofenceMonitor.Current.Geofences.Add(geofence);
                    }
                }
            }
            return await startLocating();
        }

        public async Task<Geoposition> startLocating()
        {
            App.instance.transfer.data.startApp = false;
            this.geolocator = new Geolocator { DesiredAccuracyInMeters = 0, MovementThreshold = 1 };
            this.geolocator.PositionChanged += Geolocator_PositionChanged;
            var position = await this.geolocator.GetGeopositionAsync();
            return position;
        }

        public void StopLocating()
        {
            App.instance.transfer.data.startApp = true;
            if(this.geolocator != null)
            this.geolocator.PositionChanged -= Geolocator_PositionChanged;
            this.geolocator = null;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            GeofenceMonitor.Current.GeofenceStateChanged -= Current_GeofenceStateChanged;

            base.OnNavigatingFrom(e);
        }

        private async void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            if (sender.Geofences.Any())
            {
                var reports = sender.ReadReports();

                foreach (var report in reports)
                {
                    switch (report.NewState)
                    {
                        case GeofenceState.Entered:
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                    MainGrid.Opacity = 0.10;
                                    Popup1.IsOpen = true;
                            });
                                break;
                        }
                        case GeofenceState.Exited:
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    MainGrid.Opacity = 1.0;
                                    Popup1.IsOpen = false;
                                });
                                break;
                            }
                    }
                }
            }
        }

        private async void startTracking()
        {
            MapControl1.ZoomLevel = 16;
            Geoposition x = await GetPosition();
            App.instance.transfer.data.startposition = x.Coordinate.Point;
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Geoposition x = await maphelper.currentLocation(args.Position);
                UpdateWalkedRoute(x.Coordinate.Point);
            });
        }

        private void UpdateWalkedRoute(Geopoint point)
        {
            if (App.instance.transfer.data.zoomCenter)
            {
                MapControl1.Center = point;
            }
            if (App.instance.transfer.data.drawOld && !MapControl1.MapElements.Contains(oldline) && App.instance.transfer.data.walkedRoutes.Count > 0)
            {
                this.oldline = maphelper.generateOldRoute();
                if (oldline.Path != null)
                {
                    MapControl1.MapElements.Add(oldline);
                }
            }
            if (App.instance.transfer.data.currentwalkedRoute.route.Count >= 2)
            {
                if(App.instance.transfer.data.calculatedRoute != null)
                {
                    MapControl1.MapElements.Add(App.instance.transfer.data.calculatedRoute);
                }
                MapControl1.MapElements.Add(maphelper.generateCurrentRoute());
            }
            if (App.instance.transfer.data.currentposition != null && !MapControl1.MapElements.Contains(App.instance.transfer.data.currentposition))
            {
                MapControl1.MapElements.Add(App.instance.transfer.data.currentposition);
            }
        }

        async private void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the error text box.
                        e.CheckResult();

                        // Update the UI with the completion status of the background task.
                        // The Run method of the background task sets the LocalSettings. 
                        var settings = ApplicationData.Current.LocalSettings;

                        // Get the status.
                        if (settings.Values.ContainsKey("Status"))
                        {
                           Debug.WriteLine(settings.Values["Status"].ToString());
                        }

                        // Do your app work here.

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
        }

        public async void FromToRoute(string from, string to)
        {
            App.instance.transfer.data.from = from;
            App.instance.transfer.data.to = to;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(from, MapControl1.Center);
            MapLocation from1 = result.Locations.First();
            MapControl1.Center = from1.Point;

            result = await MapLocationFinder.FindLocationsAsync(to, MapControl1.Center);

            maphelper.generateCalculatedRoute(result,from1);
            UpdateWalkedRoute(App.instance.transfer.data.currentposition.Location);
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
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            //GeofenceMonitor.Current.StatusChanged += Current_StatusChanged;
            IList<Geofence> list = GeofenceMonitor.Current.Geofences;
            List<Geofence> geofences = list.ToList();
            base.OnNavigatedTo(e);
            var value = (Tuple<string, string,string>)e.Parameter;
            if(value.Item1.Equals("createroute"))
            {
                FromToRoute(value.Item2, value.Item3);
            }
            MapControl1.MapElements.Clear();
        }

        private void PopButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainGrid.Opacity = 1.0;
            Popup1.IsOpen = false;
        }
    }
}
