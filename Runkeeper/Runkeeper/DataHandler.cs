using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace Runkeeper
{
    class DataHandler
    {
        public List<Geopoint> currentwalkedRoute;
        public List<List<Geopoint>> walkedRoutes;
        public MapIcon currentposition;
        public MapPolyline calculatedRoute;
        public Geopoint startposition;
        public string from, to;
    }
}
