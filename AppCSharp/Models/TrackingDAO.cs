using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class TrackingDAO
    {
        public long Id { get; set; }
        public long TrackerId { get; set; }
        public long TargetId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual PlaceDAO Place { get; set; }
        public virtual PlaceCheckingDAO PlaceChecking { get; set; }
        public virtual AppUserDAO Target { get; set; }
        public virtual AppUserDAO Tracker { get; set; }
    }
}
