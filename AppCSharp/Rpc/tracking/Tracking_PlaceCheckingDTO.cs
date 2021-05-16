using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.tracking
{
    public class Tracking_PlaceCheckingDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long AppUserId { get; set; }
        
        public long PlaceId { get; set; }
        
        public long PlaceCheckingStatusId { get; set; }
        
        public DateTime? CheckInAt { get; set; }
        
        public DateTime? CheckOutAt { get; set; }
        

        public Tracking_PlaceCheckingDTO() {}
        public Tracking_PlaceCheckingDTO(PlaceChecking PlaceChecking)
        {
            
            this.Id = PlaceChecking.Id;
            
            this.AppUserId = PlaceChecking.AppUserId;
            
            this.PlaceId = PlaceChecking.PlaceId;
            
            this.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
            
            this.CheckInAt = PlaceChecking.CheckInAt;
            
            this.CheckOutAt = PlaceChecking.CheckOutAt;
            
            this.Errors = PlaceChecking.Errors;
        }
    }

    public class Tracking_PlaceCheckingFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter AppUserId { get; set; }
        
        public IdFilter PlaceId { get; set; }
        
        public IdFilter PlaceCheckingStatusId { get; set; }
        
        public DateFilter CheckInAt { get; set; }
        
        public DateFilter CheckOutAt { get; set; }
        
        public PlaceCheckingOrder OrderBy { get; set; }
    }
}