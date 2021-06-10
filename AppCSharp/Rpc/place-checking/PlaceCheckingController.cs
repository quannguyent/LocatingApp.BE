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
using System.Dynamic;
using LocatingApp.Entities;
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.place_checking
{
    public partial class PlaceCheckingController : RpcController
    {
        private IAppUserService AppUserService;
        private IPlaceService PlaceService;
        private ICheckingStatusService CheckingStatusService;
        private IPlaceCheckingService PlaceCheckingService;
        private ICurrentContext CurrentContext;
        public PlaceCheckingController(
            IAppUserService AppUserService,
            IPlaceService PlaceService,
            ICheckingStatusService CheckingStatusService,
            IPlaceCheckingService PlaceCheckingService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.PlaceService = PlaceService;
            this.CheckingStatusService = CheckingStatusService;
            this.PlaceCheckingService = PlaceCheckingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PlaceCheckingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PlaceChecking_PlaceCheckingFilterDTO PlaceChecking_PlaceCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceCheckingFilter PlaceCheckingFilter = ConvertFilterDTOToFilterEntity(PlaceChecking_PlaceCheckingFilterDTO);
            PlaceCheckingFilter = await PlaceCheckingService.ToFilter(PlaceCheckingFilter);
            int count = await PlaceCheckingService.Count(PlaceCheckingFilter);
            return count;
        }

        [Route(PlaceCheckingRoute.List), HttpPost]
        public async Task<ActionResult<List<PlaceChecking_PlaceCheckingDTO>>> List([FromBody] PlaceChecking_PlaceCheckingFilterDTO PlaceChecking_PlaceCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceCheckingFilter PlaceCheckingFilter = ConvertFilterDTOToFilterEntity(PlaceChecking_PlaceCheckingFilterDTO);
            PlaceCheckingFilter = await PlaceCheckingService.ToFilter(PlaceCheckingFilter);
            List<PlaceChecking> PlaceCheckings = await PlaceCheckingService.List(PlaceCheckingFilter);
            List<PlaceChecking_PlaceCheckingDTO> PlaceChecking_PlaceCheckingDTOs = new List<PlaceChecking_PlaceCheckingDTO>();
            foreach (PlaceChecking PlaceChecking in PlaceCheckings)
            {
                var checking = new PlaceChecking_PlaceCheckingDTO(PlaceChecking);
                checking.PlaceGroupId = (await PlaceService.Get(PlaceChecking.PlaceId)).PlaceGroupId.Value;
                PlaceChecking_PlaceCheckingDTOs.Add(checking);
            }
            return PlaceChecking_PlaceCheckingDTOs;
        }

        [Route(PlaceCheckingRoute.Get), HttpPost]
        public async Task<ActionResult<PlaceChecking_PlaceCheckingDTO>> Get([FromBody]PlaceChecking_PlaceCheckingDTO PlaceChecking_PlaceCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PlaceChecking_PlaceCheckingDTO.Id))
                return Forbid();

            PlaceChecking PlaceChecking = await PlaceCheckingService.Get(PlaceChecking_PlaceCheckingDTO.Id);
            return new PlaceChecking_PlaceCheckingDTO(PlaceChecking);
        }

        private async Task<bool> HasPermission(long Id)
        {
            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter();
            PlaceCheckingFilter = await PlaceCheckingService.ToFilter(PlaceCheckingFilter);
            if (Id == 0)
            {

            }
            else
            {
                PlaceCheckingFilter.Id = new IdFilter { Equal = Id };
                int count = await PlaceCheckingService.Count(PlaceCheckingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private PlaceChecking ConvertDTOToEntity(PlaceChecking_PlaceCheckingDTO PlaceChecking_PlaceCheckingDTO)
        {
            PlaceChecking PlaceChecking = new PlaceChecking();
            PlaceChecking.Id = PlaceChecking_PlaceCheckingDTO.Id;
            PlaceChecking.AppUserId = PlaceChecking_PlaceCheckingDTO.AppUserId;
            PlaceChecking.PlaceId = PlaceChecking_PlaceCheckingDTO.PlaceId;
            PlaceChecking.PlaceCheckingStatusId = PlaceChecking_PlaceCheckingDTO.PlaceCheckingStatusId;
            PlaceChecking.CheckInAt = PlaceChecking_PlaceCheckingDTO.CheckInAt;
            PlaceChecking.CheckOutAt = PlaceChecking_PlaceCheckingDTO.CheckOutAt;
            PlaceChecking.AppUser = PlaceChecking_PlaceCheckingDTO.AppUser == null ? null : new AppUser
            {
                Id = PlaceChecking_PlaceCheckingDTO.AppUser.Id,
                Username = PlaceChecking_PlaceCheckingDTO.AppUser.Username,
                Password = PlaceChecking_PlaceCheckingDTO.AppUser.Password,
                DisplayName = PlaceChecking_PlaceCheckingDTO.AppUser.DisplayName,
                Email = PlaceChecking_PlaceCheckingDTO.AppUser.Email,
                Phone = PlaceChecking_PlaceCheckingDTO.AppUser.Phone,
            };
            PlaceChecking.Place = PlaceChecking_PlaceCheckingDTO.Place == null ? null : new Place
            {
                Id = PlaceChecking_PlaceCheckingDTO.Place.Id,
                Name = PlaceChecking_PlaceCheckingDTO.Place.Name,
                PlaceGroupId = PlaceChecking_PlaceCheckingDTO.Place.PlaceGroupId,
                Radius = PlaceChecking_PlaceCheckingDTO.Place.Radius,
                Latitude = PlaceChecking_PlaceCheckingDTO.Place.Latitude,
                Longtitude = PlaceChecking_PlaceCheckingDTO.Place.Longtitude,
            };
            PlaceChecking.PlaceCheckingStatus = PlaceChecking_PlaceCheckingDTO.PlaceCheckingStatus == null ? null : new CheckingStatus
            {
                Id = PlaceChecking_PlaceCheckingDTO.PlaceCheckingStatus.Id,
                Code = PlaceChecking_PlaceCheckingDTO.PlaceCheckingStatus.Code,
                Name = PlaceChecking_PlaceCheckingDTO.PlaceCheckingStatus.Name,
            };
            PlaceChecking.BaseLanguage = CurrentContext.Language;
            return PlaceChecking;
        }

        private PlaceCheckingFilter ConvertFilterDTOToFilterEntity(PlaceChecking_PlaceCheckingFilterDTO PlaceChecking_PlaceCheckingFilterDTO)
        {
            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter();
            PlaceCheckingFilter.Selects = PlaceCheckingSelect.ALL;
            PlaceCheckingFilter.Skip = PlaceChecking_PlaceCheckingFilterDTO.Skip;
            PlaceCheckingFilter.Take = PlaceChecking_PlaceCheckingFilterDTO.Take;
            PlaceCheckingFilter.OrderBy = PlaceChecking_PlaceCheckingFilterDTO.OrderBy;
            PlaceCheckingFilter.OrderType = PlaceChecking_PlaceCheckingFilterDTO.OrderType;

            PlaceCheckingFilter.Id = PlaceChecking_PlaceCheckingFilterDTO.Id;
            PlaceCheckingFilter.AppUserId = PlaceChecking_PlaceCheckingFilterDTO.AppUserId;
            PlaceCheckingFilter.PlaceId = PlaceChecking_PlaceCheckingFilterDTO.PlaceId;
            PlaceCheckingFilter.PlaceCheckingStatusId = PlaceChecking_PlaceCheckingFilterDTO.PlaceCheckingStatusId;
            PlaceCheckingFilter.CheckInAt = PlaceChecking_PlaceCheckingFilterDTO.CheckInAt;
            PlaceCheckingFilter.CheckOutAt = PlaceChecking_PlaceCheckingFilterDTO.CheckOutAt;
            return PlaceCheckingFilter;
        }
    }
}

