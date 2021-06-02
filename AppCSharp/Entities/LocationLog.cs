using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class LocationLog : DataEntity,  IEquatable<LocationLog>
    {
        public long Id { get; set; }
        public long? PreviousId { get; set; }
        public long AppUserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public long UpdateInterval { get; set; }
        public AppUser AppUser { get; set; }
        public LocationLog Previous { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(LocationLog other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.PreviousId != other.PreviousId) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.Latitude != other.Latitude) return false;
            if (this.Longtitude != other.Longtitude) return false;
            if (this.UpdateInterval != other.UpdateInterval) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class LocationLogFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter PreviousId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public LongFilter UpdateInterval { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<LocationLogFilter> OrFilter { get; set; }
        public LocationLogOrder OrderBy {get; set;}
        public LocationLogSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LocationLogOrder
    {
        Id = 0,
        Previous = 1,
        AppUser = 2,
        Latitude = 3,
        Longtitude = 4,
        UpdateInterval = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum LocationLogSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Previous = E._1,
        AppUser = E._2,
        Latitude = E._3,
        Longtitude = E._4,
        UpdateInterval = E._5,
    }
}
