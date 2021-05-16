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
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.place_checking
{
    public partial class PlaceCheckingController : RpcController
    {
        [Route(PlaceCheckingRoute.FilterListAppUser), HttpPost]
        public async Task<List<PlaceChecking_AppUserDTO>> FilterListAppUser([FromBody] PlaceChecking_AppUserFilterDTO PlaceChecking_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = PlaceChecking_AppUserFilterDTO.Id;
            AppUserFilter.Username = PlaceChecking_AppUserFilterDTO.Username;
            AppUserFilter.Password = PlaceChecking_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = PlaceChecking_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = PlaceChecking_AppUserFilterDTO.Email;
            AppUserFilter.Phone = PlaceChecking_AppUserFilterDTO.Phone;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<PlaceChecking_AppUserDTO> PlaceChecking_AppUserDTOs = AppUsers
                .Select(x => new PlaceChecking_AppUserDTO(x)).ToList();
            return PlaceChecking_AppUserDTOs;
        }
        [Route(PlaceCheckingRoute.FilterListPlace), HttpPost]
        public async Task<List<PlaceChecking_PlaceDTO>> FilterListPlace([FromBody] PlaceChecking_PlaceFilterDTO PlaceChecking_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceFilter PlaceFilter = new PlaceFilter();
            PlaceFilter.Skip = 0;
            PlaceFilter.Take = 20;
            PlaceFilter.OrderBy = PlaceOrder.Id;
            PlaceFilter.OrderType = OrderType.ASC;
            PlaceFilter.Selects = PlaceSelect.ALL;
            PlaceFilter.Id = PlaceChecking_PlaceFilterDTO.Id;
            PlaceFilter.Name = PlaceChecking_PlaceFilterDTO.Name;
            PlaceFilter.PlaceGroupId = PlaceChecking_PlaceFilterDTO.PlaceGroupId;
            PlaceFilter.Radius = PlaceChecking_PlaceFilterDTO.Radius;
            PlaceFilter.Latitude = PlaceChecking_PlaceFilterDTO.Latitude;
            PlaceFilter.Longtitude = PlaceChecking_PlaceFilterDTO.Longtitude;

            List<Place> Places = await PlaceService.List(PlaceFilter);
            List<PlaceChecking_PlaceDTO> PlaceChecking_PlaceDTOs = Places
                .Select(x => new PlaceChecking_PlaceDTO(x)).ToList();
            return PlaceChecking_PlaceDTOs;
        }
        [Route(PlaceCheckingRoute.FilterListCheckingStatus), HttpPost]
        public async Task<List<PlaceChecking_CheckingStatusDTO>> FilterListCheckingStatus([FromBody] PlaceChecking_CheckingStatusFilterDTO PlaceChecking_CheckingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CheckingStatusFilter CheckingStatusFilter = new CheckingStatusFilter();
            CheckingStatusFilter.Skip = 0;
            CheckingStatusFilter.Take = 20;
            CheckingStatusFilter.OrderBy = CheckingStatusOrder.Id;
            CheckingStatusFilter.OrderType = OrderType.ASC;
            CheckingStatusFilter.Selects = CheckingStatusSelect.ALL;
            CheckingStatusFilter.Id = PlaceChecking_CheckingStatusFilterDTO.Id;
            CheckingStatusFilter.Code = PlaceChecking_CheckingStatusFilterDTO.Code;
            CheckingStatusFilter.Name = PlaceChecking_CheckingStatusFilterDTO.Name;

            List<CheckingStatus> CheckingStatuses = await CheckingStatusService.List(CheckingStatusFilter);
            List<PlaceChecking_CheckingStatusDTO> PlaceChecking_CheckingStatusDTOs = CheckingStatuses
                .Select(x => new PlaceChecking_CheckingStatusDTO(x)).ToList();
            return PlaceChecking_CheckingStatusDTOs;
        }
    }
}

