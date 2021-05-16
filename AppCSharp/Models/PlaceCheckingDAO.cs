using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class PlaceCheckingDAO
    {
        public PlaceCheckingDAO()
        {
            Trackings = new HashSet<TrackingDAO>();
        }

        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingStatusId { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual PlaceDAO Place { get; set; }
        public virtual CheckingStatusDAO PlaceCheckingStatus { get; set; }
        public virtual ICollection<TrackingDAO> Trackings { get; set; }
    }
}
