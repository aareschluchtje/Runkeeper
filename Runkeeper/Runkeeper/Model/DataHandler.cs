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
        public List<double> totaldistances = new List<double>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void saveData()
        {
            walkedRoutes.Add(currentwalkedRoute);
            currentwalkedRoute = new List<DataStamp>();
            List<string> list = new List<string>();
            for (int v = 0; v < walkedRoutes.Count; v++)
            {
                list.Add("route" + "|" + totaldistances[v]);
                for (int i = 0; i < walkedRoutes[v].Count; i++)
                {
                    list.Add(walkedRoutes[v][i].location.Position.Latitude + "|" + walkedRoutes[v][i].location.Position.Longitude + "|" + walkedRoutes[v][i].time.ToString() + "|" + walkedRoutes[v][i].speed + "|" + walkedRoutes[v][i].distance);
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
                    if (!list[i].StartsWith("route"))
                    {
                        string[] items = list[i].Split('|');
                        Geopoint point = new Geopoint(new BasicGeoposition() { Latitude = Double.Parse(items[0]), Longitude = Double.Parse(items[1]) });
                        walkedRoutes[walkedRoutes.Count - 1].Add(new DataStamp(point, DateTime.Parse(items[2]), Double.Parse(items[3]), Double.Parse(items[4])));
                        System.Diagnostics.Debug.WriteLine(walkedRoutes);
                    }
                    else
                    {
                        walkedRoutes.Add(new List<DataStamp>());
                        string[] items = list[i].Split('|');
                        totaldistances.Add(Double.Parse(items[1]));
                    }
                }
                Debug.WriteLine(walkedRoutes);
            }
            totaldistances.Add(0);
        }

        public async Task<double> calculateUpdateDistance(Geopoint start, Geopoint end)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(start, end);
            MapRoute b = routeResult.Route;
            double distance = b.LengthInMeters;
            totaldistances[totaldistances.Count-1] += distance;
            return distance;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
