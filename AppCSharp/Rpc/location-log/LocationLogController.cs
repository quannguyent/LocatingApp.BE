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
using LocatingApp.Services.MLocationLog;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.location_log
{
    public partial class LocationLogController : RpcController
    {
        private IAppUserService AppUserService;
        private ILocationLogService LocationLogService;
        private ICurrentContext CurrentContext;
        public LocationLogController(
            IAppUserService AppUserService,
            ILocationLogService LocationLogService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.LocationLogService = LocationLogService;
            this.CurrentContext = CurrentContext;
        }

        [Route(LocationLogRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LocationLogFilter LocationLogFilter = ConvertFilterDTOToFilterEntityAsync(LocationLog_LocationLogFilterDTO);
            int count = await LocationLogService.Count(LocationLogFilter);
            return count;
        }

        [Route(LocationLogRoute.List), HttpPost]
        public async Task<ActionResult<List<LocationLog_LocationLogDTO>>> List([FromBody] LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LocationLogFilter LocationLogFilter = ConvertFilterDTOToFilterEntityAsync(LocationLog_LocationLogFilterDTO);
            List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);
            List<LocationLog_LocationLogDTO> LocationLog_LocationLogDTOs = LocationLogs
                .Select(c => new LocationLog_LocationLogDTO(c)).ToList();
            return LocationLog_LocationLogDTOs;
        }

        [Route(LocationLogRoute.Get), HttpPost]
        public async Task<ActionResult<LocationLog_LocationLogDTO>> Get([FromBody]LocationLog_LocationLogDTO LocationLog_LocationLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(LocationLog_LocationLogDTO.Id))
                return Forbid();

            LocationLog LocationLog = await LocationLogService.Get(LocationLog_LocationLogDTO.Id);
            return new LocationLog_LocationLogDTO(LocationLog);
        }

        [Route(LocationLogRoute.Create), HttpPost]
        public async Task<ActionResult<LocationLog_LocationLogDTO>> Create([FromBody] LocationLog_LocationLogDTO LocationLog_LocationLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(LocationLog_LocationLogDTO.Id))
                return Forbid();

            LocationLog LocationLog = ConvertDTOToEntity(LocationLog_LocationLogDTO);
            LocationLog = await LocationLogService.Create(LocationLog);
            LocationLog_LocationLogDTO = new LocationLog_LocationLogDTO(LocationLog);
            if (LocationLog.IsValidated)
                return LocationLog_LocationLogDTO;
            else
                return BadRequest(LocationLog_LocationLogDTO);
        }

        [Route(LocationLogRoute.Update), HttpPost]
        public async Task<ActionResult<LocationLog_LocationLogDTO>> Update([FromBody] LocationLog_LocationLogDTO LocationLog_LocationLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(LocationLog_LocationLogDTO.Id))
                return Forbid();

            LocationLog LocationLog = ConvertDTOToEntity(LocationLog_LocationLogDTO);
            LocationLog = await LocationLogService.Update(LocationLog);
            LocationLog_LocationLogDTO = new LocationLog_LocationLogDTO(LocationLog);
            if (LocationLog.IsValidated)
                return LocationLog_LocationLogDTO;
            else
                return BadRequest(LocationLog_LocationLogDTO);
        }

        [Route(LocationLogRoute.Delete), HttpPost]
        public async Task<ActionResult<LocationLog_LocationLogDTO>> Delete([FromBody] LocationLog_LocationLogDTO LocationLog_LocationLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(LocationLog_LocationLogDTO.Id))
                return Forbid();

            LocationLog LocationLog = ConvertDTOToEntity(LocationLog_LocationLogDTO);
            LocationLog = await LocationLogService.Delete(LocationLog);
            LocationLog_LocationLogDTO = new LocationLog_LocationLogDTO(LocationLog);
            if (LocationLog.IsValidated)
                return LocationLog_LocationLogDTO;
            else
                return BadRequest(LocationLog_LocationLogDTO);
        }
        
        [Route(LocationLogRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            LocationLogFilter LocationLogFilter = new LocationLogFilter();
            LocationLogFilter.Id = new IdFilter { In = Ids };
            LocationLogFilter.Selects = LocationLogSelect.Id;
            LocationLogFilter.Skip = 0;
            LocationLogFilter.Take = int.MaxValue;

            List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);
            LocationLogs = await LocationLogService.BulkDelete(LocationLogs);
            if (LocationLogs.Any(x => !x.IsValidated))
                return BadRequest(LocationLogs.Where(x => !x.IsValidated));
            return true;
        }

        //Import Export
        //[Route(LocationLogRoute.Import), HttpPost]
        //public async Task<ActionResult> Import(IFormFile file)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    AppUserFilter AppUserFilter = new AppUserFilter
        //    {
        //        Skip = 0,
        //        Take = int.MaxValue,
        //        Selects = AppUserSelect.ALL
        //    };
        //    List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
        //    List<LocationLog> LocationLogs = new List<LocationLog>();
        //    using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
        //    {
        //        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
        //        if (worksheet == null)
        //            return Ok(LocationLogs);
        //        int StartColumn = 1;
        //        int StartRow = 1;
        //        int IdColumn = 0 + StartColumn;
        //        int PreviousIdColumn = 1 + StartColumn;
        //        int AppUserIdColumn = 2 + StartColumn;
        //        int LatitudeColumn = 3 + StartColumn;
        //        int LongtitudeColumn = 4 + StartColumn;
        //        int UpdateIntervalColumn = 5 + StartColumn;
        //        int UsedColumn = 9 + StartColumn;

        //        for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
        //        {
        //            if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
        //                break;
        //            string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
        //            string PreviousIdValue = worksheet.Cells[i + StartRow, PreviousIdColumn].Value?.ToString();
        //            string AppUserIdValue = worksheet.Cells[i + StartRow, AppUserIdColumn].Value?.ToString();
        //            string LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
        //            string LongtitudeValue = worksheet.Cells[i + StartRow, LongtitudeColumn].Value?.ToString();
        //            string UpdateIntervalValue = worksheet.Cells[i + StartRow, UpdateIntervalColumn].Value?.ToString();
        //            string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
        //            LocationLog LocationLog = new LocationLog();
        //            LocationLog.Latitude = decimal.TryParse(LatitudeValue, out decimal Latitude) ? Latitude : 0;
        //            LocationLog.Longtitude = decimal.TryParse(LongtitudeValue, out decimal Longtitude) ? Longtitude : 0;
        //            LocationLog.UpdateInterval = long.TryParse(UpdateIntervalValue, out long UpdateInterval) ? UpdateInterval : 0;
        //            AppUser AppUser = AppUsers.Where(x => x.Id.ToString() == AppUserIdValue).FirstOrDefault();
        //            LocationLog.AppUserId = AppUser == null ? 0 : AppUser.Id;
        //            LocationLog.AppUser = AppUser;
                    
        //            LocationLogs.Add(LocationLog);
        //        }
        //    }
        //    LocationLogs = await LocationLogService.Import(LocationLogs);
        //    if (LocationLogs.All(x => x.IsValidated))
        //        return Ok(true);
        //    else
        //    {
        //        List<string> Errors = new List<string>();
        //        for (int i = 0; i < LocationLogs.Count; i++)
        //        {
        //            LocationLog LocationLog = LocationLogs[i];
        //            if (!LocationLog.IsValidated)
        //            {
        //                string Error = $"Dòng {i + 2} có lỗi:";
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.Id)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.Id)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.PreviousId)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.PreviousId)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.AppUserId)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.AppUserId)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.Latitude)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.Latitude)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.Longtitude)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.Longtitude)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.UpdateInterval)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.UpdateInterval)];
        //                if (LocationLog.Errors.ContainsKey(nameof(LocationLog.Used)))
        //                    Error += LocationLog.Errors[nameof(LocationLog.Used)];
        //                Errors.Add(Error);
        //            }
        //        }
        //        return BadRequest(Errors);
        //    }
        //}
        
        //[Route(LocationLogRoute.Export), HttpPost]
        //public async Task<ActionResult> Export([FromBody] LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
            
        //    MemoryStream memoryStream = new MemoryStream();
        //    using (ExcelPackage excel = new ExcelPackage(memoryStream))
        //    {
        //        #region LocationLog
        //        var LocationLogFilter = ConvertFilterDTOToFilterEntityAsync(LocationLog_LocationLogFilterDTO);
        //        LocationLogFilter.Skip = 0;
        //        LocationLogFilter.Take = int.MaxValue;
        //        LocationLogFilter = await LocationLogService.ToFilter(LocationLogFilter);
        //        List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);

        //        var LocationLogHeaders = new List<string[]>()
        //        {
        //            new string[] { 
        //                "Id",
        //                "PreviousId",
        //                "AppUserId",
        //                "Latitude",
        //                "Longtitude",
        //                "UpdateInterval",
        //                "Used",
        //            }
        //        };
        //        List<object[]> LocationLogData = new List<object[]>();
        //        for (int i = 0; i < LocationLogs.Count; i++)
        //        {
        //            var LocationLog = LocationLogs[i];
        //            LocationLogData.Add(new Object[]
        //            {
        //                LocationLog.Id,
        //                LocationLog.PreviousId,
        //                LocationLog.AppUserId,
        //                LocationLog.Latitude,
        //                LocationLog.Longtitude,
        //                LocationLog.UpdateInterval,
        //                LocationLog.Used,
        //            });
        //        }
        //        excel.GenerateWorksheet("LocationLog", LocationLogHeaders, LocationLogData);
        //        #endregion
                
        //        #region AppUser
        //        var AppUserFilter = new AppUserFilter();
        //        AppUserFilter.Selects = AppUserSelect.ALL;
        //        AppUserFilter.OrderBy = AppUserOrder.Id;
        //        AppUserFilter.OrderType = OrderType.ASC;
        //        AppUserFilter.Skip = 0;
        //        AppUserFilter.Take = int.MaxValue;
        //        List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

        //        var AppUserHeaders = new List<string[]>()
        //        {
        //            new string[] { 
        //                "Id",
        //                "Username",
        //                "Password",
        //                "DisplayName",
        //                "Email",
        //                "Phone",
        //                "Used",
        //            }
        //        };
        //        List<object[]> AppUserData = new List<object[]>();
        //        for (int i = 0; i < AppUsers.Count; i++)
        //        {
        //            var AppUser = AppUsers[i];
        //            AppUserData.Add(new Object[]
        //            {
        //                AppUser.Id,
        //                AppUser.Username,
        //                AppUser.Password,
        //                AppUser.DisplayName,
        //                AppUser.Email,
        //                AppUser.Phone,
        //                AppUser.Used,
        //            });
        //        }
        //        excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
        //        #endregion
        //        excel.Save();
        //    }
        //    return File(memoryStream.ToArray(), "application/octet-stream", "LocationLog.xlsx");
        //}

        //[Route(LocationLogRoute.ExportTemplate), HttpPost]
        //public async Task<ActionResult> ExportTemplate([FromBody] LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
            
        //    string path = "Templates/LocationLog_Template.xlsx";
        //    byte[] arr = System.IO.File.ReadAllBytes(path);
        //    MemoryStream input = new MemoryStream(arr);
        //    MemoryStream output = new MemoryStream();
        //    dynamic Data = new ExpandoObject();
        //    using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
        //    {
        //        document.Process(Data);
        //    };
        //    return File(output.ToArray(), "application/octet-stream", "LocationLog.xlsx");
        //}

        private async Task<bool> HasPermission(long Id)
        {
            LocationLogFilter LocationLogFilter = new LocationLogFilter
            {
                AppUserId = new IdFilter { Equal = CurrentContext.UserId }
            };
            if (Id == 0)
            {

            }
            else
            {
                LocationLogFilter.Id = new IdFilter { Equal = Id };
                int count = await LocationLogService.Count(LocationLogFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private LocationLog ConvertDTOToEntity(LocationLog_LocationLogDTO LocationLog_LocationLogDTO)
        {
            LocationLog LocationLog = new LocationLog();
            LocationLog.Id = LocationLog_LocationLogDTO.Id;
            LocationLog.PreviousId = LocationLog_LocationLogDTO.PreviousId;
            LocationLog.AppUserId = LocationLog_LocationLogDTO.AppUserId;
            LocationLog.Latitude = LocationLog_LocationLogDTO.Latitude;
            LocationLog.Longtitude = LocationLog_LocationLogDTO.Longtitude;
            LocationLog.UpdateInterval = LocationLog_LocationLogDTO.UpdateInterval;
            LocationLog.AppUser = LocationLog_LocationLogDTO.AppUser == null ? null : new AppUser
            {
                Id = LocationLog_LocationLogDTO.AppUser.Id,
                Username = LocationLog_LocationLogDTO.AppUser.Username,
                Password = LocationLog_LocationLogDTO.AppUser.Password,
                DisplayName = LocationLog_LocationLogDTO.AppUser.DisplayName,
                Email = LocationLog_LocationLogDTO.AppUser.Email,
                Phone = LocationLog_LocationLogDTO.AppUser.Phone,
            };
            LocationLog.Previous = LocationLog_LocationLogDTO.Previous == null ? null : new LocationLog
            {
                Id = LocationLog_LocationLogDTO.Previous.Id,
                PreviousId = LocationLog_LocationLogDTO.Previous.PreviousId,
                AppUserId = LocationLog_LocationLogDTO.Previous.AppUserId,
                Latitude = LocationLog_LocationLogDTO.Previous.Latitude,
                Longtitude = LocationLog_LocationLogDTO.Previous.Longtitude,
                UpdateInterval = LocationLog_LocationLogDTO.Previous.UpdateInterval,
            };
            LocationLog.BaseLanguage = CurrentContext.Language;
            return LocationLog;
        }

        private LocationLogFilter ConvertFilterDTOToFilterEntityAsync(LocationLog_LocationLogFilterDTO LocationLog_LocationLogFilterDTO)
        {
            LocationLogFilter LocationLogFilter = new LocationLogFilter();
            LocationLogFilter.Selects = LocationLogSelect.ALL;
            LocationLogFilter.Skip = LocationLog_LocationLogFilterDTO.Skip;
            LocationLogFilter.Take = LocationLog_LocationLogFilterDTO.Take;
            LocationLogFilter.OrderBy = LocationLog_LocationLogFilterDTO.OrderBy;
            LocationLogFilter.OrderType = LocationLog_LocationLogFilterDTO.OrderType;

            LocationLogFilter.Id = LocationLog_LocationLogFilterDTO.Id;
            LocationLogFilter.PreviousId = LocationLog_LocationLogFilterDTO.PreviousId;
            LocationLogFilter.AppUserId = LocationLog_LocationLogFilterDTO.AppUserId;
            LocationLogFilter.Latitude = LocationLog_LocationLogFilterDTO.Latitude;
            LocationLogFilter.Longtitude = LocationLog_LocationLogFilterDTO.Longtitude;
            LocationLogFilter.UpdateInterval = LocationLog_LocationLogFilterDTO.UpdateInterval;
            LocationLogFilter.CreatedAt = LocationLog_LocationLogFilterDTO.CreatedAt;
            LocationLogFilter.UpdatedAt = LocationLog_LocationLogFilterDTO.UpdatedAt;
            return LocationLogFilter;
        }
    }
}

