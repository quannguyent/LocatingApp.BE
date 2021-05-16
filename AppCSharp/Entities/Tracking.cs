using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class Tracking : DataEntity,  IEquatable<Tracking>
    {
        public long Id { get; set; }
        public long TrackerId { get; set; }
        public long TargetId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingId { get; set; }
        public bool Used { get; set; }
        public Place Place { get; set; }
        public PlaceChecking PlaceChecking { get; set; }
        public AppUser Target { get; set; }
        public AppUser Tracker { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Tracking other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.TrackerId != other.TrackerId) return false;
            if (this.TargetId != other.TargetId) return false;
            if (this.PlaceId != other.PlaceId) return false;
            if (this.PlaceCheckingId != other.PlaceCheckingId) return false;
            if (this.Used != other.Used) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class TrackingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter TrackerId { get; set; }
        public IdFilter TargetId { get; set; }
        public IdFilter PlaceId { get; set; }
        public IdFilter PlaceCheckingId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<TrackingFilter> OrFilter { get; set; }
        public TrackingOrder OrderBy {get; set;}
        public TrackingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TrackingOrder
    {
        Id = 0,
        Tracker = 1,
        Target = 2,
        Place = 3,
        PlaceChecking = 4,
        Used = 8,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum TrackingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Tracker = E._1,
        Target = E._2,
        Place = E._3,
        PlaceChecking = E._4,
        Used = E._8,
    }
}
