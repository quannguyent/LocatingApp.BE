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
using LocatingApp.Services.MLocationLog;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.location_log
{
    public partial class LocationLogController : RpcController
    {
        [Route(LocationLogRoute.SingleListAppUser), HttpPost]
        public async Task<List<LocationLog_AppUserDTO>> SingleListAppUser([FromBody] LocationLog_AppUserFilterDTO LocationLog_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = LocationLog_AppUserFilterDTO.Id;
            AppUserFilter.Username = LocationLog_AppUserFilterDTO.Username;
            AppUserFilter.Password = LocationLog_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = LocationLog_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = LocationLog_AppUserFilterDTO.Email;
            AppUserFilter.Phone = LocationLog_AppUserFilterDTO.Phone;
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<LocationLog_AppUserDTO> LocationLog_AppUserDTOs = AppUsers
                .Select(x => new LocationLog_AppUserDTO(x)).ToList();
            return LocationLog_AppUserDTOs;
        }
        [Route(LocationLogRoute.SingleListLocationLog), HttpPost]
        public async Task<List<LocationLog_LocationLogDTO>> SingleListLocationLog([FromBody] LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LocationLogFilter LocationLogFilter = new LocationLogFilter();
            LocationLogFilter.Skip = 0;
            LocationLogFilter.Take = 20;
            LocationLogFilter.OrderBy = LocationLogOrder.Id;
            LocationLogFilter.OrderType = OrderType.ASC;
            LocationLogFilter.Selects = LocationLogSelect.ALL;
            LocationLogFilter.Id = LocationLog_LocationLogFilterDTO.Id;
            LocationLogFilter.PreviousId = LocationLog_LocationLogFilterDTO.PreviousId;
            LocationLogFilter.AppUserId = LocationLog_LocationLogFilterDTO.AppUserId;
            LocationLogFilter.Latitude = LocationLog_LocationLogFilterDTO.Latitude;
            LocationLogFilter.Longtitude = LocationLog_LocationLogFilterDTO.Longtitude;
            LocationLogFilter.UpdateInterval = LocationLog_LocationLogFilterDTO.UpdateInterval;
            List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);
            List<LocationLog_LocationLogDTO> LocationLog_LocationLogDTOs = LocationLogs
                .Select(x => new LocationLog_LocationLogDTO(x)).ToList();
            return LocationLog_LocationLogDTOs;
        }
    }
}

