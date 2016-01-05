using System;
using System.Collections.Generic;
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

        public static async Task saveData()
        {
            walkedRoutes.Add(currentwalkedRoute);
            currentwalkedRoute = new List<Geopoint>();
            Stream responseStream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("something.txt", CreationCollisionOption.ReplaceExisting);
            if(responseStream.CanWrite)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<List<Geopoint>>));
                serializer.WriteObject(responseStream, walkedRoutes);
                await responseStream.FlushAsync();
            }
            responseStream.Dispose();
        }

        public static async Task loadData()
        {
            string iets = "\\something.txt";
            if(File.Exists(ApplicationData.Current.LocalFolder.Path + iets))
            {
                Stream responseStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("something.txt");
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<List<Geopoint>>));
                walkedRoutes = (List<List<Geopoint>>)serializer.ReadObject(responseStream);
                await responseStream.FlushAsync();
                responseStream.Dispose();
            }
            else
            {
                await saveData();
            }
            
        }
    }
}
