using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper
{
    static class DataHandler
    {
        static public List<Geopoint> currentwalkedRoute = new List<Geopoint>();
        static public List<List<Geopoint>> walkedRoutes = new List<List<Geopoint>>();
        static public MapIcon currentposition;
        static public MapPolyline calculatedRoute;
        static public Geopoint startposition;
        static public string from, to;

        public static void saveData()
        {
            walkedRoutes.Add(currentwalkedRoute);
            currentwalkedRoute = new List<Geopoint>();
            List<string> list = new List<string>();
            foreach (List<Geopoint> route in walkedRoutes)
            {
                list.Add("route");
                foreach(Geopoint point in route)
                {
                    list.Add(point.Position.Latitude + "|" + point.Position.Longitude);
                }
            }
            File.WriteAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt", list);
        }

        public static void loadData()
        {
            if(File.Exists(ApplicationData.Current.LocalFolder.Path + "//something.txt"))
            {
                walkedRoutes = new List<List<Geopoint>>();
                string[] list = File.ReadAllLines(ApplicationData.Current.LocalFolder.Path + "//something.txt");
                for (int i = 0; i < list.Length; i++)
                {
                    if(!list[i].Equals("route"))
                    {
                        string[] items = list[i].Split('|');
                        walkedRoutes[walkedRoutes.Count - 1].Add(new Geopoint(new BasicGeoposition() { Latitude = Double.Parse(items[0]), Longitude = Double.Parse(items[1]) }));
                    }
                    else
                    {
                        walkedRoutes.Add(new List<Geopoint>());
                    }
                }
                Debug.WriteLine(walkedRoutes);
            }
        }
    }
}
