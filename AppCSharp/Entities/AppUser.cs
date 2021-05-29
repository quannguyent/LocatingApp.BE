using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class AppUser : DataEntity,  IEquatable<AppUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpired { get; set; }
        public string Token { get; set; }
        public bool Used { get; set; }
        public List<LocationLog> LocationLogs { get; set; }
        public Sex Sex { get; set; }
        public List<AppUserAppUserMapping> AppUserAppUserMappingAppUsers { get; set; }
        public List<AppUserAppUserMapping> AppUserAppUserMappingFriends { get; set; }
        public List<PlaceChecking> PlaceCheckings { get; set; }
        public List<Tracking> TrackingTargets { get; set; }
        public List<Tracking> TrackingTrackers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool Equals(AppUser other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Username != other.Username) return false;
            if (this.Password != other.Password) return false;
            if (this.DisplayName != other.DisplayName) return false;
            if (this.SexId != other.SexId) return false;
            if (this.Email != other.Email) return false;
            if (this.Phone != other.Phone) return false;
            if (this.Used != other.Used) return false;
            if (this.LocationLogs?.Count != other.LocationLogs?.Count) return false;
            else if (this.LocationLogs != null && other.LocationLogs != null)
            {
                for (int i = 0; i < LocationLogs.Count; i++)
                {
                    LocationLog LocationLog = LocationLogs[i];
                    LocationLog otherLocationLog = other.LocationLogs[i];
                    if (LocationLog == null && otherLocationLog != null)
                        return false;
                    if (LocationLog != null && otherLocationLog == null)
                        return false;
                    if (LocationLog.Equals(otherLocationLog) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public IdFilter SexId { get; set; }
        public DateFilter Birthday { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<AppUserFilter> OrFilter { get; set; }
        public AppUserOrder OrderBy {get; set;}
        public AppUserSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserOrder
    {
        Id = 0,
        Username = 1,
        Password = 2,
        DisplayName = 3,
        Sex = 4,
        Birthday = 5,
        Email = 6,
        Phone = 7,
        Used = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum AppUserSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        Password = E._2,
        DisplayName = E._3,
        Sex = E._4,
        Birthday = E._5,
        Email = E._6,
        Phone = E._7,
        Used = E._9,
    }
}
