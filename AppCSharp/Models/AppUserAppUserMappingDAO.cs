using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class AppUserAppUserMappingDAO
    {
        public long AppUserId { get; set; }
        public long FriendId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual AppUserDAO Friend { get; set; }
    }
}
