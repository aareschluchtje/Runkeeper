using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Runkeeper
{
    public class DataStamp
    {
        public DataStamp(Geopoint geopoint, DateTime time, double speed, double distance)
        {
            this.location = geopoint;
            this.time = time;
            this.speed = speed;
            this.distance = distance;
        }

        public Geopoint location { get; set; }
        public DateTime time { get; set; }
        public double speed { get; set; }
        public double distance { get; set; }
    }
}
