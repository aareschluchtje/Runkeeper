using Runkeeper.Model;
using Runkeeper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper
{
    public class DataHandler : INotifyPropertyChanged
    {
        public Route currentwalkedRoute = new Route(DateTime.Now, new ObservableCollection<DataStamp>(), 0);
        public ObservableCollection<Route> walkedRoutes { get; set; }
        public MapIcon currentposition;
        public MapPolyline calculatedRoute;
        public Geopoint startposition;
        public string from, to;
        public string currentDistance { get; set; }
        public string currentSpeed { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public bool zoomCenter = true, drawOld = true;
        public bool startApp = true;
        public Time time = new Time();
        public DataHandler()
        {
            this.walkedRoutes = new ObservableCollection<Route>();
            currentDistance = "0";
            currentSpeed = "0";
        }

        public void saveData()
        {
            currentwalkedRoute.totalDistance = double.Parse(currentDistance);
            walkedRoutes.Add(currentwalkedRoute);
            currentDistance = "0";
            currentwalkedRoute = new Route(DateTime.Now, new ObservableCollection<DataStamp>(), 0);
            List<string> list = new List<string>();
            for (int v = 0; v < walkedRoutes.Count; v++)
            {
                list.Add("route" + "|" + walkedRoutes[v].totalDistance + "|" + walkedRoutes[v].date.ToString());
                for (int i = 0; i < walkedRoutes[v].route.Count; i++)
                {
                    list.Add(walkedRoutes[v].route[i].location.Position.Latitude + "|" + walkedRoutes[v].route[i].location.Position.Longitude + "|"
                    + walkedRoutes[v].route[i].time.ToString() + "|" + walkedRoutes[v].route[i].speed + "|" + walkedRoutes[v].route[i].distance);
                }
            }
            File.WriteAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt", list);
        }

        public void loadData()
        {
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + "//something.txt"))
            {
                walkedRoutes = new ObservableCollection<Route>();
                string[] list = File.ReadAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt");
                for (int i = 0; i < list.Length; i++)
                {
                    if (!list[i].StartsWith("route"))
                    {
                        string[] items = list[i].Split('|');
                        Geopoint point = new Geopoint(new BasicGeoposition() { Latitude = Double.Parse(items[0]), Longitude = Double.Parse(items[1]) });
                        walkedRoutes[walkedRoutes.Count - 1].route.Add(new DataStamp(point, DateTime.Parse(items[2]), Double.Parse(items[3]), Double.Parse(items[4])));
                    }
                    else
                    {
                        walkedRoutes.Add(new Route(DateTime.Now, new ObservableCollection<DataStamp>(), 0));
                        string[] items = list[i].Split('|');
                        walkedRoutes[walkedRoutes.Count-1].totalDistance = Double.Parse(items[1]);
                        walkedRoutes[walkedRoutes.Count - 1].date = DateTime.Parse(items[2]);
                    }
                }
            }
        }

        public string speedChanges(string speed)
        {
            for(int i = 0; i < currentwalkedRoute.route.Count; i++)
            {
                if (currentwalkedRoute.route.Count != 0)
                {
                    DataStamp item = currentwalkedRoute.route[currentwalkedRoute.route.Count - 1];
                    currentSpeed = item.speed.ToString();
                    currentSpeed = speed;
                    NotifyPropertyChanged(nameof(currentSpeed));
                }
                else
                {
                    currentSpeed = "0";
                }
            }

            return currentSpeed;
        }

        public async Task<double> calculateUpdateDistance(Geopoint start, Geopoint end)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(start, end);
            double distance = 0;
            if(routeResult.Route != null)
            {
                MapRoute b = routeResult.Route;
                distance = b.LengthInMeters;
                currentDistance = (double.Parse(currentDistance) + distance).ToString();
            }
            NotifyPropertyChanged(nameof(currentDistance));
            return distance;

        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool getZoomCenter()
        {
            return zoomCenter;
        }

        public void setZoomCenter(bool zoomCenter)
        {
            this.zoomCenter = zoomCenter;
        }

        public bool getDrawOld()
        {
            return drawOld;
        }

        public void setDrawOld(bool drawOld)
        {
            this.drawOld = drawOld;
        }
    }
}
