using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;

namespace Runkeeper.Model
{
    public class GeofenceHandler
    {

        static public Geofence createGeofence(Geopoint location)
        {
            // Set the fence ID.
            string fenceId = "fence" + location.Position.Latitude + location.Position.Longitude;

            BasicGeoposition position = new BasicGeoposition { Latitude = location.Position.Latitude, Longitude = location.Position.Longitude };
            // Define the fence location and radius.
            double radius = 10; // in meters

            // Set a circular region for the geofence.
            Geocircle geocircle = new Geocircle(position, radius);

            bool singleUse = false;

            MonitoredGeofenceStates mask = 
            MonitoredGeofenceStates.Entered |
            MonitoredGeofenceStates.Exited;

            TimeSpan dwellTime = TimeSpan.FromSeconds(1);
            TimeSpan duration = TimeSpan.FromDays(1);
            DateTimeOffset startTime = DateTime.Now;
            // Create the geofence.
            Geofence geofence = new Geofence(fenceId, geocircle, mask, singleUse, dwellTime, startTime, duration);
            return geofence;
        }
    }
}
