using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Runkeeper
{
    class DataStamp
    {
        public DataStamp(Geopoint geopoint, DateTime time, double speed)
        {
            this.location = geopoint;
        }

        public Geopoint location { get; set; }
        public DateTime time { get; set; }
        public double speed { get; set; }
    }
}
