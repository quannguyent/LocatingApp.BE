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
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MLocationLog;

namespace LocatingApp.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        [Route(AppUserRoute.FilterListLocationLog), HttpPost]
        public async Task<List<AppUser_LocationLogDTO>> FilterListLocationLog([FromBody] AppUser_LocationLogFilterDTO AppUser_LocationLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LocationLogFilter LocationLogFilter = new LocationLogFilter();
            LocationLogFilter.Skip = 0;
            LocationLogFilter.Take = 20;
            LocationLogFilter.OrderBy = LocationLogOrder.Id;
            LocationLogFilter.OrderType = OrderType.ASC;
            LocationLogFilter.Selects = LocationLogSelect.ALL;
            LocationLogFilter.Id = AppUser_LocationLogFilterDTO.Id;
            LocationLogFilter.AppUserId = AppUser_LocationLogFilterDTO.AppUserId;
            LocationLogFilter.Latitude = AppUser_LocationLogFilterDTO.Latitude;
            LocationLogFilter.Longtitude = AppUser_LocationLogFilterDTO.Longtitude;
            LocationLogFilter.UpdateInterval = AppUser_LocationLogFilterDTO.UpdateInterval;

            List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);
            List<AppUser_LocationLogDTO> AppUser_LocationLogDTOs = LocationLogs
                .Select(x => new AppUser_LocationLogDTO(x)).ToList();
            return AppUser_LocationLogDTOs;
        }
    }
}

