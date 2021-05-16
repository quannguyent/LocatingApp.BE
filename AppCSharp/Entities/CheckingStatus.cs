using System;
using System.Collections.Generic;
using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocatingApp.Entities
{
    public class CheckingStatus : DataEntity,  IEquatable<CheckingStatus>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(CheckingStatus other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CheckingStatusFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<CheckingStatusFilter> OrFilter { get; set; }
        public CheckingStatusOrder OrderBy {get; set;}
        public CheckingStatusSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CheckingStatusOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum CheckingStatusSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
