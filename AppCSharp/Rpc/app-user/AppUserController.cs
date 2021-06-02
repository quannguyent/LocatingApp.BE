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
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MLocationLog;

namespace LocatingApp.Rpc.app_user
{
    public partial class AppUserController : RpcController
    {
        private ILocationLogService LocationLogService;
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(
            ILocationLogService LocationLogService,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext
        )
        {
            this.LocationLogService = LocationLogService;
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }

        [Route(AppUserRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            int count = await AppUserService.Count(AppUserFilter);
            return count;
        }

        [Route(AppUserRoute.List), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> List([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = AppUsers
                .Select(c => new AppUser_AppUserDTO(c)).ToList();
            return AppUser_AppUserDTOs;
        }

        [Route(AppUserRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Get([FromBody]AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = await AppUserService.Get(AppUser_AppUserDTO.Id);
            return new AppUser_AppUserDTO(AppUser);
        }

        [Route(AppUserRoute.Create), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Create([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Update([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(AppUserRoute.Delete), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Delete([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(AppUser_AppUserDTO.Id))
                return Forbid();

            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser = await AppUserService.Delete(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }
        
        [Route(AppUserRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = new IdFilter { In = Ids };
            AppUserFilter.Selects = AppUserSelect.Id;
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            AppUsers = await AppUserService.BulkDelete(AppUsers);
            if (AppUsers.Any(x => !x.IsValidated))
                return BadRequest(AppUsers.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(AppUserRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<AppUser> AppUsers = new List<AppUser>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(AppUsers);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UsernameColumn = 1 + StartColumn;
                int PasswordColumn = 2 + StartColumn;
                int DisplayNameColumn = 3 + StartColumn;
                int EmailColumn = 4 + StartColumn;
                int PhoneColumn = 5 + StartColumn;
                int UsedColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string UsernameValue = worksheet.Cells[i + StartRow, UsernameColumn].Value?.ToString();
                    string PasswordValue = worksheet.Cells[i + StartRow, PasswordColumn].Value?.ToString();
                    string DisplayNameValue = worksheet.Cells[i + StartRow, DisplayNameColumn].Value?.ToString();
                    string EmailValue = worksheet.Cells[i + StartRow, EmailColumn].Value?.ToString();
                    string PhoneValue = worksheet.Cells[i + StartRow, PhoneColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    AppUser AppUser = new AppUser();
                    AppUser.Username = UsernameValue;
                    AppUser.Password = PasswordValue;
                    AppUser.DisplayName = DisplayNameValue;
                    AppUser.Email = EmailValue;
                    AppUser.Phone = PhoneValue;
                    
                    AppUsers.Add(AppUser);
                }
            }
            AppUsers = await AppUserService.Import(AppUsers);
            if (AppUsers.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    AppUser AppUser = AppUsers[i];
                    if (!AppUser.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Id)))
                            Error += AppUser.Errors[nameof(AppUser.Id)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Username)))
                            Error += AppUser.Errors[nameof(AppUser.Username)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Password)))
                            Error += AppUser.Errors[nameof(AppUser.Password)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.DisplayName)))
                            Error += AppUser.Errors[nameof(AppUser.DisplayName)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Email)))
                            Error += AppUser.Errors[nameof(AppUser.Email)];
                        if (AppUser.Errors.ContainsKey(nameof(AppUser.Phone)))
                            Error += AppUser.Errors[nameof(AppUser.Phone)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(AppUserRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region AppUser
                var AppUserFilter = ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO);
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "Password",
                        "DisplayName",
                        "Email",
                        "Phone",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.Password,
                        AppUser.DisplayName,
                        AppUser.Email,
                        AppUser.Phone,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                
                #region LocationLog
                var LocationLogFilter = new LocationLogFilter();
                LocationLogFilter.Selects = LocationLogSelect.ALL;
                LocationLogFilter.OrderBy = LocationLogOrder.Id;
                LocationLogFilter.OrderType = OrderType.ASC;
                LocationLogFilter.Skip = 0;
                LocationLogFilter.Take = int.MaxValue;
                List<LocationLog> LocationLogs = await LocationLogService.List(LocationLogFilter);

                var LocationLogHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "PreviousId",
                        "AppUserId",
                        "Latitude",
                        "Longtitude",
                        "UpdateInterval",
                    }
                };
                List<object[]> LocationLogData = new List<object[]>();
                for (int i = 0; i < LocationLogs.Count; i++)
                {
                    var LocationLog = LocationLogs[i];
                    LocationLogData.Add(new Object[]
                    {
                        LocationLog.Id,
                        LocationLog.PreviousId,
                        LocationLog.AppUserId,
                        LocationLog.Latitude,
                        LocationLog.Longtitude,
                        LocationLog.UpdateInterval,
                    });
                }
                excel.GenerateWorksheet("LocationLog", LocationLogHeaders, LocationLogData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        [Route(AppUserRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/AppUser_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "AppUser.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            if (Id == 0)
            {

            }
            else
            {
                AppUserFilter.Id = new IdFilter { Equal = Id };
                int count = await AppUserService.Count(AppUserFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.LocationLogs = AppUser_AppUserDTO.LocationLogs?
                .Select(x => new LocationLog
                {
                    Id = x.Id,
                    PreviousId = x.PreviousId,
                    Latitude = x.Latitude,
                    Longtitude = x.Longtitude,
                    UpdateInterval = x.UpdateInterval,
                    Previous = x.Previous == null ? null : new LocationLog
                    {
                        Id = x.Previous.Id,
                        PreviousId = x.Previous.PreviousId,
                        AppUserId = x.Previous.AppUserId,
                        Latitude = x.Previous.Latitude,
                        Longtitude = x.Previous.Longtitude,
                        UpdateInterval = x.Previous.UpdateInterval,
                    },
                }).ToList();
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }

        private AppUserFilter ConvertFilterDTOToFilterEntity(AppUser_AppUserFilterDTO AppUser_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Skip = AppUser_AppUserFilterDTO.Skip;
            AppUserFilter.Take = AppUser_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUser_AppUserFilterDTO.OrderBy;
            AppUserFilter.OrderType = AppUser_AppUserFilterDTO.OrderType;

            AppUserFilter.Id = AppUser_AppUserFilterDTO.Id;
            AppUserFilter.Username = AppUser_AppUserFilterDTO.Username;
            AppUserFilter.Password = AppUser_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = AppUser_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = AppUser_AppUserFilterDTO.Email;
            AppUserFilter.Phone = AppUser_AppUserFilterDTO.Phone;
            AppUserFilter.CreatedAt = AppUser_AppUserFilterDTO.CreatedAt;
            AppUserFilter.UpdatedAt = AppUser_AppUserFilterDTO.UpdatedAt;
            return AppUserFilter;
        }
    }
}

