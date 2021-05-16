using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class PlaceChecking : DataEntity,  IEquatable<PlaceChecking>
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingStatusId { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public AppUser AppUser { get; set; }
        public Place Place { get; set; }
        public CheckingStatus PlaceCheckingStatus { get; set; }
        
        public bool Equals(PlaceChecking other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.PlaceId != other.PlaceId) return false;
            if (this.PlaceCheckingStatusId != other.PlaceCheckingStatusId) return false;
            if (this.CheckInAt != other.CheckInAt) return false;
            if (this.CheckOutAt != other.CheckOutAt) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PlaceCheckingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter PlaceId { get; set; }
        public IdFilter PlaceCheckingStatusId { get; set; }
        public DateFilter CheckInAt { get; set; }
        public DateFilter CheckOutAt { get; set; }
        public List<PlaceCheckingFilter> OrFilter { get; set; }
        public PlaceCheckingOrder OrderBy {get; set;}
        public PlaceCheckingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaceCheckingOrder
    {
        Id = 0,
        AppUser = 1,
        Place = 2,
        PlaceCheckingStatus = 3,
        CheckInAt = 4,
        CheckOutAt = 5,
    }

    [Flags]
    public enum PlaceCheckingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        AppUser = E._1,
        Place = E._2,
        PlaceCheckingStatus = E._3,
        CheckInAt = E._4,
        CheckOutAt = E._5,
    }
}
