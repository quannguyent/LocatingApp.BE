using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Entities
{
    public class PlaceSearchParams
    {
        public string Text { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLon { get; set; }
        public decimal BBoxMinLon { get; set; }
        public decimal BBoxMaxLon { get; set; }
        public decimal BBoxMinLat { get; set; }
        public decimal BBoxMaxLat { get; set; }
        public long Size { get; set; }
        public string Categories { get; set; }
    }
}
