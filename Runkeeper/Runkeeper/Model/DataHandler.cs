using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper
{
    public class DataHandler : INotifyPropertyChanged
    {
        public List<DataStamp> currentwalkedRoute = new List<DataStamp>();
        public List<List<DataStamp>> walkedRoutes = new List<List<DataStamp>>();
        public Geolocator geolocator;
        public MapIcon currentposition;
        public MapPolyline calculatedRoute;
        public Geopoint startposition;
        public string from, to;
        public double totaldistance;

        public event PropertyChangedEventHandler PropertyChanged;

        public void saveData()
        {
            walkedRoutes.Add(currentwalkedRoute);
            currentwalkedRoute = new List<DataStamp>();
            List<string> list = new List<string>();
            foreach (List<DataStamp> route in walkedRoutes)
            {
                list.Add("route");
                for (int i = 0; i < route.Count; i++)
                {
                    list.Add(route[i].location.Position.Latitude + "|" + route[i].location.Position.Longitude + "|" + route[i].time.ToString() + "|" + route[i].speed + "|" + route[i].distance);
                }
            }
            File.WriteAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt", list);
        }

        public void loadData()
        {
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + "//something.txt"))
            {
                walkedRoutes = new List<List<DataStamp>>();
                string[] list = File.ReadAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt");
                for (int i = 0; i < list.Length; i++)
                {
                    if (!list[i].Equals("route"))
                    {
                        string[] items = list[i].Split('|');
                        Geopoint point = new Geopoint(new BasicGeoposition() { Latitude = Double.Parse(items[0]), Longitude = Double.Parse(items[1]) });
                        walkedRoutes[walkedRoutes.Count - 1].Add(new DataStamp(point, DateTime.Parse(items[2]), Double.Parse(items[3]), Double.Parse(items[4])));
                        System.Diagnostics.Debug.WriteLine(walkedRoutes);
                    }
                    else
                    {
                        walkedRoutes.Add(new List<DataStamp>());
                    }
                }
                Debug.WriteLine(walkedRoutes);
            }
        }

        public async Task<double> calculateDistance(Geopoint start, Geopoint end)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(start, end);
            MapRoute b = routeResult.Route;
            double distance = b.LengthInMeters;
            totaldistance += distance;
            return distance;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
