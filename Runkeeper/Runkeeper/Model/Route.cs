using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Runkeeper.Annotations;

namespace Runkeeper.Model
{
    public class Route
    {
        public DateTime date { get; set; }
        public ObservableCollection<DataStamp> route { get; set; }
        public double totalDistance { get; set; }

        public Route(DateTime date, ObservableCollection<DataStamp> route, double totalDistance)
        {
            this.date = date;
            this.route = route;
            this.totalDistance = totalDistance;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", totalDistance,date);
        }
    }
}
