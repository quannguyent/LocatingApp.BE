using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserAppUserMappingAppUsers = new HashSet<AppUserAppUserMappingDAO>();
            AppUserAppUserMappingFriends = new HashSet<AppUserAppUserMappingDAO>();
            LocationLogs = new HashSet<LocationLogDAO>();
            PlaceCheckings = new HashSet<PlaceCheckingDAO>();
            TrackingTargets = new HashSet<TrackingDAO>();
            TrackingTrackers = new HashSet<TrackingDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }

        public virtual ICollection<AppUserAppUserMappingDAO> AppUserAppUserMappingAppUsers { get; set; }
        public virtual ICollection<AppUserAppUserMappingDAO> AppUserAppUserMappingFriends { get; set; }
        public virtual ICollection<LocationLogDAO> LocationLogs { get; set; }
        public virtual ICollection<PlaceCheckingDAO> PlaceCheckings { get; set; }
        public virtual ICollection<TrackingDAO> TrackingTargets { get; set; }
        public virtual ICollection<TrackingDAO> TrackingTrackers { get; set; }
    }
}
