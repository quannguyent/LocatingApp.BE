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
            List<PlaceChecking_PlaceCheckingDTO> PlaceChecking_PlaceCheckingDTOs = PlaceCheckings
                .Select(c => new PlaceChecking_PlaceCheckingDTO(c)).ToList();
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

        [Route(PlaceCheckingRoute.Create), HttpPost]
        public async Task<ActionResult<PlaceChecking_PlaceCheckingDTO>> Create([FromBody] PlaceChecking_PlaceCheckingDTO PlaceChecking_PlaceCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PlaceChecking_PlaceCheckingDTO.Id))
                return Forbid();

            PlaceChecking PlaceChecking = ConvertDTOToEntity(PlaceChecking_PlaceCheckingDTO);
            PlaceChecking = await PlaceCheckingService.Create(PlaceChecking);
            PlaceChecking_PlaceCheckingDTO = new PlaceChecking_PlaceCheckingDTO(PlaceChecking);
            if (PlaceChecking.IsValidated)
                return PlaceChecking_PlaceCheckingDTO;
            else
                return BadRequest(PlaceChecking_PlaceCheckingDTO);
        }

        [Route(PlaceCheckingRoute.Update), HttpPost]
        public async Task<ActionResult<PlaceChecking_PlaceCheckingDTO>> Update([FromBody] PlaceChecking_PlaceCheckingDTO PlaceChecking_PlaceCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PlaceChecking_PlaceCheckingDTO.Id))
                return Forbid();

            PlaceChecking PlaceChecking = ConvertDTOToEntity(PlaceChecking_PlaceCheckingDTO);
            PlaceChecking = await PlaceCheckingService.Update(PlaceChecking);
            PlaceChecking_PlaceCheckingDTO = new PlaceChecking_PlaceCheckingDTO(PlaceChecking);
            if (PlaceChecking.IsValidated)
                return PlaceChecking_PlaceCheckingDTO;
            else
                return BadRequest(PlaceChecking_PlaceCheckingDTO);
        }

        [Route(PlaceCheckingRoute.Delete), HttpPost]
        public async Task<ActionResult<PlaceChecking_PlaceCheckingDTO>> Delete([FromBody] PlaceChecking_PlaceCheckingDTO PlaceChecking_PlaceCheckingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PlaceChecking_PlaceCheckingDTO.Id))
                return Forbid();

            PlaceChecking PlaceChecking = ConvertDTOToEntity(PlaceChecking_PlaceCheckingDTO);
            PlaceChecking = await PlaceCheckingService.Delete(PlaceChecking);
            PlaceChecking_PlaceCheckingDTO = new PlaceChecking_PlaceCheckingDTO(PlaceChecking);
            if (PlaceChecking.IsValidated)
                return PlaceChecking_PlaceCheckingDTO;
            else
                return BadRequest(PlaceChecking_PlaceCheckingDTO);
        }
        
        [Route(PlaceCheckingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter();
            PlaceCheckingFilter = await PlaceCheckingService.ToFilter(PlaceCheckingFilter);
            PlaceCheckingFilter.Id = new IdFilter { In = Ids };
            PlaceCheckingFilter.Selects = PlaceCheckingSelect.Id;
            PlaceCheckingFilter.Skip = 0;
            PlaceCheckingFilter.Take = int.MaxValue;

            List<PlaceChecking> PlaceCheckings = await PlaceCheckingService.List(PlaceCheckingFilter);
            PlaceCheckings = await PlaceCheckingService.BulkDelete(PlaceCheckings);
            if (PlaceCheckings.Any(x => !x.IsValidated))
                return BadRequest(PlaceCheckings.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(PlaceCheckingRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            PlaceFilter PlaceFilter = new PlaceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PlaceSelect.ALL
            };
            List<Place> Places = await PlaceService.List(PlaceFilter);
            CheckingStatusFilter PlaceCheckingStatusFilter = new CheckingStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = CheckingStatusSelect.ALL
            };
            List<CheckingStatus> PlaceCheckingStatuses = await CheckingStatusService.List(PlaceCheckingStatusFilter);
            List<PlaceChecking> PlaceCheckings = new List<PlaceChecking>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(PlaceCheckings);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int AppUserIdColumn = 1 + StartColumn;
                int PlaceIdColumn = 2 + StartColumn;
                int PlaceCheckingStatusIdColumn = 3 + StartColumn;
                int CheckInAtColumn = 4 + StartColumn;
                int CheckOutAtColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string AppUserIdValue = worksheet.Cells[i + StartRow, AppUserIdColumn].Value?.ToString();
                    string PlaceIdValue = worksheet.Cells[i + StartRow, PlaceIdColumn].Value?.ToString();
                    string PlaceCheckingStatusIdValue = worksheet.Cells[i + StartRow, PlaceCheckingStatusIdColumn].Value?.ToString();
                    string CheckInAtValue = worksheet.Cells[i + StartRow, CheckInAtColumn].Value?.ToString();
                    string CheckOutAtValue = worksheet.Cells[i + StartRow, CheckOutAtColumn].Value?.ToString();
                    
                    PlaceChecking PlaceChecking = new PlaceChecking();
                    PlaceChecking.CheckInAt = DateTime.TryParse(CheckInAtValue, out DateTime CheckInAt) ? CheckInAt : DateTime.Now;
                    PlaceChecking.CheckOutAt = DateTime.TryParse(CheckOutAtValue, out DateTime CheckOutAt) ? CheckOutAt : DateTime.Now;
                    AppUser AppUser = AppUsers.Where(x => x.Id.ToString() == AppUserIdValue).FirstOrDefault();
                    PlaceChecking.AppUserId = AppUser == null ? 0 : AppUser.Id;
                    PlaceChecking.AppUser = AppUser;
                    Place Place = Places.Where(x => x.Id.ToString() == PlaceIdValue).FirstOrDefault();
                    PlaceChecking.PlaceId = Place == null ? 0 : Place.Id;
                    PlaceChecking.Place = Place;
                    CheckingStatus PlaceCheckingStatus = PlaceCheckingStatuses.Where(x => x.Id.ToString() == PlaceCheckingStatusIdValue).FirstOrDefault();
                    PlaceChecking.PlaceCheckingStatusId = PlaceCheckingStatus == null ? 0 : PlaceCheckingStatus.Id;
                    PlaceChecking.PlaceCheckingStatus = PlaceCheckingStatus;
                    
                    PlaceCheckings.Add(PlaceChecking);
                }
            }
            PlaceCheckings = await PlaceCheckingService.Import(PlaceCheckings);
            if (PlaceCheckings.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < PlaceCheckings.Count; i++)
                {
                    PlaceChecking PlaceChecking = PlaceCheckings[i];
                    if (!PlaceChecking.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.Id)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.Id)];
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.AppUserId)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.AppUserId)];
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.PlaceId)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.PlaceId)];
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.PlaceCheckingStatusId)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.PlaceCheckingStatusId)];
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.CheckInAt)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.CheckInAt)];
                        if (PlaceChecking.Errors.ContainsKey(nameof(PlaceChecking.CheckOutAt)))
                            Error += PlaceChecking.Errors[nameof(PlaceChecking.CheckOutAt)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(PlaceCheckingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] PlaceChecking_PlaceCheckingFilterDTO PlaceChecking_PlaceCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PlaceChecking
                var PlaceCheckingFilter = ConvertFilterDTOToFilterEntity(PlaceChecking_PlaceCheckingFilterDTO);
                PlaceCheckingFilter.Skip = 0;
                PlaceCheckingFilter.Take = int.MaxValue;
                PlaceCheckingFilter = await PlaceCheckingService.ToFilter(PlaceCheckingFilter);
                List<PlaceChecking> PlaceCheckings = await PlaceCheckingService.List(PlaceCheckingFilter);

                var PlaceCheckingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "AppUserId",
                        "PlaceId",
                        "PlaceCheckingStatusId",
                        "CheckInAt",
                        "CheckOutAt",
                    }
                };
                List<object[]> PlaceCheckingData = new List<object[]>();
                for (int i = 0; i < PlaceCheckings.Count; i++)
                {
                    var PlaceChecking = PlaceCheckings[i];
                    PlaceCheckingData.Add(new Object[]
                    {
                        PlaceChecking.Id,
                        PlaceChecking.AppUserId,
                        PlaceChecking.PlaceId,
                        PlaceChecking.PlaceCheckingStatusId,
                        PlaceChecking.CheckInAt,
                        PlaceChecking.CheckOutAt,
                    });
                }
                excel.GenerateWorksheet("PlaceChecking", PlaceCheckingHeaders, PlaceCheckingData);
                #endregion
                
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
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
                        "Used",
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
                        AppUser.Used,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region Place
                var PlaceFilter = new PlaceFilter();
                PlaceFilter.Selects = PlaceSelect.ALL;
                PlaceFilter.OrderBy = PlaceOrder.Id;
                PlaceFilter.OrderType = OrderType.ASC;
                PlaceFilter.Skip = 0;
                PlaceFilter.Take = int.MaxValue;
                List<Place> Places = await PlaceService.List(PlaceFilter);

                var PlaceHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "PlaceGroupId",
                        "Radius",
                        "Latitude",
                        "Longtitude",
                        "Used",
                    }
                };
                List<object[]> PlaceData = new List<object[]>();
                for (int i = 0; i < Places.Count; i++)
                {
                    var Place = Places[i];
                    PlaceData.Add(new Object[]
                    {
                        Place.Id,
                        Place.Name,
                        Place.PlaceGroupId,
                        Place.Radius,
                        Place.Latitude,
                        Place.Longtitude,
                        Place.Used,
                    });
                }
                excel.GenerateWorksheet("Place", PlaceHeaders, PlaceData);
                #endregion
                #region CheckingStatus
                var CheckingStatusFilter = new CheckingStatusFilter();
                CheckingStatusFilter.Selects = CheckingStatusSelect.ALL;
                CheckingStatusFilter.OrderBy = CheckingStatusOrder.Id;
                CheckingStatusFilter.OrderType = OrderType.ASC;
                CheckingStatusFilter.Skip = 0;
                CheckingStatusFilter.Take = int.MaxValue;
                List<CheckingStatus> CheckingStatuses = await CheckingStatusService.List(CheckingStatusFilter);

                var CheckingStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> CheckingStatusData = new List<object[]>();
                for (int i = 0; i < CheckingStatuses.Count; i++)
                {
                    var CheckingStatus = CheckingStatuses[i];
                    CheckingStatusData.Add(new Object[]
                    {
                        CheckingStatus.Id,
                        CheckingStatus.Code,
                        CheckingStatus.Name,
                    });
                }
                excel.GenerateWorksheet("CheckingStatus", CheckingStatusHeaders, CheckingStatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "PlaceChecking.xlsx");
        }

        [Route(PlaceCheckingRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] PlaceChecking_PlaceCheckingFilterDTO PlaceChecking_PlaceCheckingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/PlaceChecking_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "PlaceChecking.xlsx");
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
                Used = PlaceChecking_PlaceCheckingDTO.AppUser.Used,
            };
            PlaceChecking.Place = PlaceChecking_PlaceCheckingDTO.Place == null ? null : new Place
            {
                Id = PlaceChecking_PlaceCheckingDTO.Place.Id,
                Name = PlaceChecking_PlaceCheckingDTO.Place.Name,
                PlaceGroupId = PlaceChecking_PlaceCheckingDTO.Place.PlaceGroupId,
                Radius = PlaceChecking_PlaceCheckingDTO.Place.Radius,
                Latitude = PlaceChecking_PlaceCheckingDTO.Place.Latitude,
                Longtitude = PlaceChecking_PlaceCheckingDTO.Place.Longtitude,
                Used = PlaceChecking_PlaceCheckingDTO.Place.Used,
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

