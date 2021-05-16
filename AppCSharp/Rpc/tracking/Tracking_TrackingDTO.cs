using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.tracking
{
    public class Tracking_TrackingDTO : DataDTO
    {
        public long Id { get; set; }
        public long TrackerId { get; set; }
        public long TargetId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingId { get; set; }
        public bool Used { get; set; }
        public Tracking_PlaceDTO Place { get; set; }
        public Tracking_PlaceCheckingDTO PlaceChecking { get; set; }
        public Tracking_AppUserDTO Target { get; set; }
        public Tracking_AppUserDTO Tracker { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Tracking_TrackingDTO() {}
        public Tracking_TrackingDTO(Tracking Tracking)
        {
            this.Id = Tracking.Id;
            this.TrackerId = Tracking.TrackerId;
            this.TargetId = Tracking.TargetId;
            this.PlaceId = Tracking.PlaceId;
            this.PlaceCheckingId = Tracking.PlaceCheckingId;
            this.Used = Tracking.Used;
            this.Place = Tracking.Place == null ? null : new Tracking_PlaceDTO(Tracking.Place);
            this.PlaceChecking = Tracking.PlaceChecking == null ? null : new Tracking_PlaceCheckingDTO(Tracking.PlaceChecking);
            this.Target = Tracking.Target == null ? null : new Tracking_AppUserDTO(Tracking.Target);
            this.Tracker = Tracking.Tracker == null ? null : new Tracking_AppUserDTO(Tracking.Tracker);
            this.CreatedAt = Tracking.CreatedAt;
            this.UpdatedAt = Tracking.UpdatedAt;
            this.Errors = Tracking.Errors;
        }
    }

    public class Tracking_TrackingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter TrackerId { get; set; }
        public IdFilter TargetId { get; set; }
        public IdFilter PlaceId { get; set; }
        public IdFilter PlaceCheckingId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public TrackingOrder OrderBy { get; set; }
    }
}
