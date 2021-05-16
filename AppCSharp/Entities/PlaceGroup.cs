using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class PlaceGroup : DataEntity,  IEquatable<PlaceGroup>
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Used { get; set; }
        public PlaceGroup Parent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(PlaceGroup other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ParentId != other.ParentId) return false;
            if (this.Name != other.Name) return false;
            if (this.Code != other.Code) return false;
            if (this.Used != other.Used) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PlaceGroupFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Code { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<PlaceGroupFilter> OrFilter { get; set; }
        public PlaceGroupOrder OrderBy {get; set;}
        public PlaceGroupSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlaceGroupOrder
    {
        Id = 0,
        Parent = 1,
        Name = 2,
        Code = 3,
        Used = 7,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum PlaceGroupSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Parent = E._1,
        Name = E._2,
        Code = E._3,
        Used = E._7,
    }
}
