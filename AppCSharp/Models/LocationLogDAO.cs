using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class LocationLogDAO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public long UpdateInterval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
    }
}
