using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class PlaceDAO
    {
        public PlaceDAO()
        {
            PlaceCheckings = new HashSet<PlaceCheckingDAO>();
            Trackings = new HashSet<TrackingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long? PlaceGroupId { get; set; }
        public long Radius { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<PlaceCheckingDAO> PlaceCheckings { get; set; }
        public virtual ICollection<TrackingDAO> Trackings { get; set; }
    }
}
