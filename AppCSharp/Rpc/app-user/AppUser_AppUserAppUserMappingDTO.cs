using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.app_user
{
    public class AppUser_AppUserAppUserMappingDTO : DataDTO
    {
        public long AppUserId { get; set; }
        public long FriendId { get; set; }
        public bool Used { get; set; }
        public AppUser AppUser { get; set; }
        public AppUser Friend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public AppUser_AppUserAppUserMappingDTO() { }
        public AppUser_AppUserAppUserMappingDTO(AppUserAppUserMapping AppUserAppUserMapping)
        {
            this.AppUserId = AppUserAppUserMapping.AppUserId;
            this.FriendId = AppUserAppUserMapping.FriendId;
            this.AppUser = AppUserAppUserMapping.Friend;
            this.Friend = AppUserAppUserMapping.Friend;
            this.CreatedAt = AppUserAppUserMapping.CreatedAt;
            this.UpdatedAt = AppUserAppUserMapping.UpdatedAt;
            this.Errors = AppUserAppUserMapping.Errors;
        }
    }

    public class AppUser_AppUserAppUserMappingFilterDTO : FilterDTO
    {
        public IdFilter UserId { get; set; }
        public IdFilter FriendId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
