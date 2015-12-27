using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
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
        }

        public static void LoadData()
        {

        }
    }
}
