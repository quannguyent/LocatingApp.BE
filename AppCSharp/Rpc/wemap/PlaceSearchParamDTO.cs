using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Rpc.wemap
{
    public class PlaceSearchParamDTO
    {
        public string Text { get; set; }
        public decimal Rad { get; set; }
        public long Size { get; set; }
        public long CategoryIds { get; set; }
    }
}
