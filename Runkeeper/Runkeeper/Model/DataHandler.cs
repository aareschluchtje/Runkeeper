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
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper
{
    public class DataHandler : INotifyPropertyChanged
    {
        public ObservableCollection<DataStamp> currentwalkedRoute = new ObservableCollection<DataStamp>();
        public ObservableCollection<ObservableCollection<DataStamp>> walkedRoutes = new ObservableCollection<ObservableCollection<DataStamp>>();
        public Geolocator geolocator;
        public MapIcon currentposition;
        public MapPolyline calculatedRoute;
        public Geopoint startposition;
        public string from, to;
        public List<double> totaldistances = new List<double>();
        public string currentDistance { get; set; }
        public string currentSpeed { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public DataHandler()
        {
            currentDistance = "0";
            currentSpeed = "0";
        }

        public ObservableCollection<DataStamp> ItemsCollection
        {
            get { return currentwalkedRoute; }
        }

        public void saveData()
        {
            walkedRoutes.Add(currentwalkedRoute);
            totaldistances.Add(double.Parse(currentDistance));
            currentDistance = "0";
            currentwalkedRoute = new ObservableCollection<DataStamp>();
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
                walkedRoutes = new ObservableCollection<ObservableCollection<DataStamp>>();
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
                        walkedRoutes.Add(new ObservableCollection<DataStamp>());
                        string[] items = list[i].Split('|');
                        totaldistances.Add(Double.Parse(items[1]));
                    }
                }
                Debug.WriteLine(walkedRoutes);
            }
        }

        public string speedChanges(string speed)
        {
            for(int i = 0; i < currentwalkedRoute.Count; i++)
            {
                if (currentwalkedRoute.Count != 0)
                {
                    DataStamp item = currentwalkedRoute[currentwalkedRoute.Count - 1];
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
	    Debug.WriteLine(start + " " + end);
            if(routeResult.Route != null)
            {
                MapRoute b = routeResult.Route;
                distance = b.LengthInMeters;
                currentDistance = (double.Parse(currentDistance) + distance).ToString();
            }
            NotifyPropertyChanged(nameof(currentDistance));
            System.Diagnostics.Debug.WriteLine("Afstand " + currentDistance);
            return distance;

        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
