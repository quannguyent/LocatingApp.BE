using GeoCoordinatePortable;
using LocatingApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Helpers
{
    public class Coordinate
    {
        public static decimal GetDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var sCoord = new GeoCoordinate((double)lat1 - 90, (double)lon1 - 90);
            var eCoord = new GeoCoordinate((double)lat2 - 90, (double)lon2 - 90);

            decimal result = (decimal)sCoord.GetDistanceTo(eCoord);
            return result;
        }
    }
}
