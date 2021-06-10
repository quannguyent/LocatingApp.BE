using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.place_checking
{
    public class PlaceChecking_PlaceCheckingDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long PlaceId { get; set; }
        public long PlaceCheckingStatusId { get; set; }
        public long PlaceGroupId { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public PlaceChecking_AppUserDTO AppUser { get; set; }
        public PlaceChecking_PlaceDTO Place { get; set; }
        public PlaceChecking_CheckingStatusDTO PlaceCheckingStatus { get; set; }
        public PlaceChecking_PlaceCheckingDTO() {}
        public PlaceChecking_PlaceCheckingDTO(PlaceChecking PlaceChecking)
        {
            this.Id = PlaceChecking.Id;
            this.AppUserId = PlaceChecking.AppUserId;
            this.PlaceId = PlaceChecking.PlaceId;
            this.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
            this.CheckInAt = PlaceChecking.CheckInAt;
            this.CheckOutAt = PlaceChecking.CheckOutAt;
            this.AppUser = PlaceChecking.AppUser == null ? null : new PlaceChecking_AppUserDTO(PlaceChecking.AppUser);
            this.Place = PlaceChecking.Place == null ? null : new PlaceChecking_PlaceDTO(PlaceChecking.Place);
            this.PlaceCheckingStatus = PlaceChecking.PlaceCheckingStatus == null ? null : new PlaceChecking_CheckingStatusDTO(PlaceChecking.PlaceCheckingStatus);
            this.Errors = PlaceChecking.Errors;
        }
    }

    public class PlaceChecking_PlaceCheckingFilterDTO : FilterDTO
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
