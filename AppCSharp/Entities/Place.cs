using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class Place : DataEntity,  IEquatable<Place>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long? PlaceGroupId { get; set; }
        public long Radius { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<PlaceChecking> PlaceCheckings { get; set; }

        public bool Equals(Place other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.PlaceGroupId != other.PlaceGroupId) return false;
            if (this.Radius != other.Radius) return false;
            if (this.Latitude != other.Latitude) return false;
            if (this.Longtitude != other.Longtitude) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PlaceFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter PlaceGroupId { get; set; }
        public LongFilter Radius { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<PlaceFilter> OrFilter { get; set; }
        public PlaceOrder OrderBy {get; set;}
        public PlaceSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaceOrder
    {
        Id = 0,
        Name = 1,
        Code = 6,
        PlaceGroup = 2,
        Radius = 3,
        Latitude = 4,
        Longtitude = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum PlaceSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Code = E._6,
        PlaceGroup = E._2,
        Radius = E._3,
        Latitude = E._4,
        Longtitude = E._5,
    }
}
