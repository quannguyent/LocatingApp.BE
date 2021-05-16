using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.tracking
{
    public class Tracking_PlaceDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long? PlaceGroupId { get; set; }
        
        public long Radius { get; set; }
        
        public decimal Latitude { get; set; }
        
        public decimal Longtitude { get; set; }
        
        public bool Used { get; set; }
        

        public Tracking_PlaceDTO() {}
        public Tracking_PlaceDTO(Place Place)
        {
            
            this.Id = Place.Id;
            
            this.Name = Place.Name;
            
            this.PlaceGroupId = Place.PlaceGroupId;
            
            this.Radius = Place.Radius;
            
            this.Latitude = Place.Latitude;
            
            this.Longtitude = Place.Longtitude;
            
            this.Used = Place.Used;
            
            this.Errors = Place.Errors;
        }
    }

    public class Tracking_PlaceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter PlaceGroupId { get; set; }
        
        public LongFilter Radius { get; set; }
        
        public DecimalFilter Latitude { get; set; }
        
        public DecimalFilter Longtitude { get; set; }
        
        public PlaceOrder OrderBy { get; set; }
    }
}