using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper.Model
{
    public class MapHelper: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static async Task<MapLocation> FindLocation(string location, Geopoint reference)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(location, reference);
            MapLocation from = result.Locations.First();
            return from;
        }

        public static async Task<MapRoute> FindRunnerRoute(Geopoint from, Geopoint to)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(from, to);
            MapRoute b = routeResult.Route;
            return b;
        }

        public async Task<Geoposition> currentLocation(Geoposition position)
        {
            if (App.instance.transfer.data.currentposition == null)
            {
                App.instance.transfer.data.currentposition = new MapIcon();
                App.instance.transfer.data.currentposition.ZIndex = 3;
                App.instance.transfer.data.currentposition.NormalizedAnchorPoint = new Point(0.5, 1.0);
                App.instance.transfer.data.currentposition.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MapIcon.png"));
            }
            App.instance.transfer.data.currentposition.Location = position.Coordinate.Point;
            double speed = Double.Parse(App.instance.transfer.data.speedChanges(position.Coordinate.Speed.ToString()));
            NotifyPropertyChanged(nameof(App.instance.transfer.data.currentSpeed));

            if (App.instance.transfer.data.currentwalkedRoute.route.Count != 0)
            {
                double distance = await App.instance.transfer.data.calculateUpdateDistance(App.instance.transfer.data.currentwalkedRoute.route[App.instance.transfer.data.currentwalkedRoute.route.Count - 1].location, position.Coordinate.Point);
                App.instance.transfer.data.currentwalkedRoute.route.Add(new DataStamp(position.Coordinate.Point, DateTime.Now, speed, distance));
            }
            else
            {
                App.instance.transfer.data.currentwalkedRoute.route.Add(new DataStamp(position.Coordinate.Point, DateTime.Now, 0, 0));
            }

            return position;
        }

        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public MapPolyline generateOldRoute()
        {
            List<BasicGeoposition> oldpositions = new List<BasicGeoposition>();
            foreach (Route route in App.instance.transfer.data.walkedRoutes)
            {
                for (int i = 0; i < route.route.Count; i++)
                {
                    oldpositions.Add(new BasicGeoposition() { Latitude = route.route[i].location.Position.Latitude, Longitude = route.route[i].location.Position.Longitude });
                }
            }
            MapPolyline oldline = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = Colors.Gray,
                StrokeDashed = false,
                ZIndex = 1
            };
            oldline.Path = new Geopath(oldpositions);
            return oldline;
        }

        public MapPolyline generateCurrentRoute()
        {
            MapPolyline currentline = new MapPolyline
            {
                StrokeThickness = 11,
                StrokeColor = Colors.Blue,
                StrokeDashed = false,
                ZIndex = 2
            };
            List<BasicGeoposition> positions = new List<BasicGeoposition>();
            for (int i = 0; i < App.instance.transfer.data.currentwalkedRoute.route.Count; i++)
            {
                positions.Add(new BasicGeoposition() { Latitude = App.instance.transfer.data.currentwalkedRoute.route[i].location.Position.Latitude, Longitude = App.instance.transfer.data.currentwalkedRoute.route[i].location.Position.Longitude });
            }
            currentline.Path = new Geopath(positions);
            return currentline;
        }

        public async void generateCalculatedRoute(MapLocationFinderResult result, MapLocation from1)
        {
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
    }
}
