using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class AppUserAppUserMapping : DataEntity, IEquatable<AppUserAppUserMapping>
    {
        public long AppUserId { get; set; }
        public long FriendId { get; set; }
        public AppUser AppUser { get; set; }
        public AppUser Friend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(AppUserAppUserMapping other)
        {
            if (other == null) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.FriendId != other.FriendId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserAppUserMappingFilter : FilterEntity
    {
        public IdFilter AppUserId { get; set; }
        public IdFilter FriendId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<AppUserAppUserMappingFilter> OrFilter { get; set; }
        public AppUserAppUserMappingOrder OrderBy {get; set;}
        public AppUserAppUserMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserAppUserMappingOrder
    {
        AppUser = 0,
        Friend = 1,
        Used = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum AppUserAppUserMappingSelect:long
    {
        ALL = E.ALL,
        AppUser = E._0,
        Friend = E._1,
        Used = E._5,
    }
}
