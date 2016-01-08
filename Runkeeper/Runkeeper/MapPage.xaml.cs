﻿using System;
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
        public MapPage()
        {
            this.InitializeComponent();
            DataHandler.currentwalkedRoute = new List<DataStamp>();
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
            DataHandler.startposition = x.Coordinate.Point;
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentLocation(args.Position);
            });
        }

        private void currentLocation(Geoposition position)
        {
            MapControl1.Center = position.Coordinate.Point;
            DataHandler.currentposition = new MapIcon();
            DataHandler.currentposition.Location = position.Coordinate.Point;
            DataHandler.currentposition.Title = "I am here";

            DataHandler.currentposition.ZIndex = 3;

            DataHandler.currentwalkedRoute.Add(new DataStamp(position.Coordinate.Point, DateTime.Now, 0));

            UpdateWalkedRoute();
        }

        private void UpdateWalkedRoute()
        {
            if (DataHandler.currentwalkedRoute.Count >= 2)
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
                for (int i = 0; i < DataHandler.currentwalkedRoute.Count; i++)
                {
                    positions.Add(new BasicGeoposition() { Latitude = DataHandler.currentwalkedRoute[i].location.Position.Latitude, Longitude = DataHandler.currentwalkedRoute[i].location.Position.Longitude });
                }
                List<BasicGeoposition> oldpositions = new List<BasicGeoposition>();
                foreach (List<DataStamp> route in DataHandler.walkedRoutes)
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
                if(DataHandler.calculatedRoute != null)
                {
                    MapControl1.MapElements.Add(DataHandler.calculatedRoute);
                }
                MapControl1.MapElements.Add(currentline);

                if(oldline.Path != null)
                {
                    MapControl1.MapElements.Add(oldline);
                }
                MapControl1.MapElements.Add(DataHandler.currentposition);
            }
        }

        public async void FromToRoute(string from, string to)
        {
            DataHandler.from = from;
            DataHandler.to = to;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(from, MapControl1.Center);
            MapLocation from1 = result.Locations.First();

            result = await MapLocationFinder.FindLocationsAsync(to, MapControl1.Center);

            MapLocation to1 = result.Locations.First();
            MapRouteFinderResult routeresult = await MapRouteFinder.GetWalkingRouteAsync(from1.Point, to1.Point);

            MapRoute map1 = routeresult.Route;

            var color = Colors.Red;
            DataHandler.calculatedRoute = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = color,
                StrokeDashed = false,
                ZIndex = 2
            };
            DataHandler.calculatedRoute.Path = new Geopath(map1.Path.Positions);
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
            Frame.Navigate(typeof(CreateRoute), new Tuple<string, string>(DataHandler.from,DataHandler.to));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var value = (Tuple<string, string,string>)e.Parameter;
            if(value.Item1.Equals("createroute"))
            {
                FromToRoute(value.Item2, value.Item3);
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            startTracking();
        }
    }
}
