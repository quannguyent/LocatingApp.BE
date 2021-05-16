using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class AggregatedCounterDAO
    {
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
