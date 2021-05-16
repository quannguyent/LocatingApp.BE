using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using LocatingApp.Entities;
using LocatingApp.Services.MTracking;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.tracking
{
    public partial class TrackingController : RpcController
    {
        [Route(TrackingRoute.FilterListPlace), HttpPost]
        public async Task<List<Tracking_PlaceDTO>> FilterListPlace([FromBody] Tracking_PlaceFilterDTO Tracking_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceFilter PlaceFilter = new PlaceFilter();
            PlaceFilter.Skip = 0;
            PlaceFilter.Take = 20;
            PlaceFilter.OrderBy = PlaceOrder.Id;
            PlaceFilter.OrderType = OrderType.ASC;
            PlaceFilter.Selects = PlaceSelect.ALL;
            PlaceFilter.Id = Tracking_PlaceFilterDTO.Id;
            PlaceFilter.Name = Tracking_PlaceFilterDTO.Name;
            PlaceFilter.PlaceGroupId = Tracking_PlaceFilterDTO.PlaceGroupId;
            PlaceFilter.Radius = Tracking_PlaceFilterDTO.Radius;
            PlaceFilter.Latitude = Tracking_PlaceFilterDTO.Latitude;
            PlaceFilter.Longtitude = Tracking_PlaceFilterDTO.Longtitude;

            List<Place> Places = await PlaceService.List(PlaceFilter);
            List<Tracking_PlaceDTO> Tracking_PlaceDTOs = Places
                .Select(x => new Tracking_PlaceDTO(x)).ToList();
            return Tracking_PlaceDTOs;
        }
        [Route(TrackingRoute.FilterListPlaceChecking), HttpPost]
        public async Task<List<Tracking_PlaceCheckingDTO>> FilterListPlaceChecking([FromBody] Tracking_PlaceCheckingFilterDTO Tracking_PlaceCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter();
            PlaceCheckingFilter.Skip = 0;
            PlaceCheckingFilter.Take = 20;
            PlaceCheckingFilter.OrderBy = PlaceCheckingOrder.Id;
            PlaceCheckingFilter.OrderType = OrderType.ASC;
            PlaceCheckingFilter.Selects = PlaceCheckingSelect.ALL;
            PlaceCheckingFilter.Id = Tracking_PlaceCheckingFilterDTO.Id;
            PlaceCheckingFilter.AppUserId = Tracking_PlaceCheckingFilterDTO.AppUserId;
            PlaceCheckingFilter.PlaceId = Tracking_PlaceCheckingFilterDTO.PlaceId;
            PlaceCheckingFilter.PlaceCheckingStatusId = Tracking_PlaceCheckingFilterDTO.PlaceCheckingStatusId;
            PlaceCheckingFilter.CheckInAt = Tracking_PlaceCheckingFilterDTO.CheckInAt;
            PlaceCheckingFilter.CheckOutAt = Tracking_PlaceCheckingFilterDTO.CheckOutAt;

            List<PlaceChecking> PlaceCheckings = await PlaceCheckingService.List(PlaceCheckingFilter);
            List<Tracking_PlaceCheckingDTO> Tracking_PlaceCheckingDTOs = PlaceCheckings
                .Select(x => new Tracking_PlaceCheckingDTO(x)).ToList();
            return Tracking_PlaceCheckingDTOs;
        }
        [Route(TrackingRoute.FilterListAppUser), HttpPost]
        public async Task<List<Tracking_AppUserDTO>> FilterListAppUser([FromBody] Tracking_AppUserFilterDTO Tracking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = Tracking_AppUserFilterDTO.Id;
            AppUserFilter.Username = Tracking_AppUserFilterDTO.Username;
            AppUserFilter.Password = Tracking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = Tracking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = Tracking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = Tracking_AppUserFilterDTO.Phone;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<Tracking_AppUserDTO> Tracking_AppUserDTOs = AppUsers
                .Select(x => new Tracking_AppUserDTO(x)).ToList();
            return Tracking_AppUserDTOs;
        }
    }
}

