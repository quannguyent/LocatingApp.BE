using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.location_log
{
    public class LocationLog_LocationLogDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public long UpdateInterval { get; set; }
        public bool Used { get; set; }
        public LocationLog_AppUserDTO AppUser { get; set; }
        public LocationLog_LocationLogDTO Previous { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public LocationLog_LocationLogDTO() {}
        public LocationLog_LocationLogDTO(LocationLog LocationLog)
        {
            this.Id = LocationLog.Id;
            this.AppUserId = LocationLog.AppUserId;
            this.Latitude = LocationLog.Latitude;
            this.Longtitude = LocationLog.Longtitude;
            this.UpdateInterval = LocationLog.UpdateInterval;
            this.AppUser = LocationLog.AppUser == null ? null : new LocationLog_AppUserDTO(LocationLog.AppUser);
            this.CreatedAt = LocationLog.CreatedAt;
            this.Errors = LocationLog.Errors;
        }
    }

    public class LocationLog_LocationLogFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public LongFilter UpdateInterval { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public LocationLogOrder OrderBy { get; set; }
    }
}
