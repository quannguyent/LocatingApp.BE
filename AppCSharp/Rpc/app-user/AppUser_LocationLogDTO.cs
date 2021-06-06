using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.app_user
{
    public class AppUser_LocationLogDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public long UpdateInterval { get; set; }
        public AppUser_LocationLogDTO Previous { get; set; }   

        public AppUser_LocationLogDTO() {}
        public AppUser_LocationLogDTO(LocationLog LocationLog)
        {
            this.Id = LocationLog.Id;
            this.AppUserId = LocationLog.AppUserId;
            this.Latitude = LocationLog.Latitude;
            this.Longtitude = LocationLog.Longtitude;
            this.UpdateInterval = LocationLog.UpdateInterval;
            this.Errors = LocationLog.Errors;
        }
    }

    public class AppUser_LocationLogFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
                
        public IdFilter AppUserId { get; set; }
        
        public DecimalFilter Latitude { get; set; }
        
        public DecimalFilter Longtitude { get; set; }
        
        public LongFilter UpdateInterval { get; set; }
        
        public LocationLogOrder OrderBy { get; set; }
    }
}