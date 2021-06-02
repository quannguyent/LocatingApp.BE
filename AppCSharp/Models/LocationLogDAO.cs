using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class LocationLogDAO
    {
        public LocationLogDAO()
        {
            InversePrevious = new HashSet<LocationLogDAO>();
        }

        public long Id { get; set; }
        public long? PreviousId { get; set; }
        public long AppUserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public long UpdateInterval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual LocationLogDAO Previous { get; set; }
        public virtual ICollection<LocationLogDAO> InversePrevious { get; set; }
    }
}
