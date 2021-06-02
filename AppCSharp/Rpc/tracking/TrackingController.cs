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
using LocatingApp.Services.MTracking;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.tracking
{
    public partial class TrackingController : RpcController
    {
        private IPlaceService PlaceService;
        private IPlaceCheckingService PlaceCheckingService;
        private IAppUserService AppUserService;
        private ITrackingService TrackingService;
        private ICurrentContext CurrentContext;
        public TrackingController(
            IPlaceService PlaceService,
            IPlaceCheckingService PlaceCheckingService,
            IAppUserService AppUserService,
            ITrackingService TrackingService,
            ICurrentContext CurrentContext
        )
        {
            this.PlaceService = PlaceService;
            this.PlaceCheckingService = PlaceCheckingService;
            this.AppUserService = AppUserService;
            this.TrackingService = TrackingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(TrackingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Tracking_TrackingFilterDTO Tracking_TrackingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TrackingFilter TrackingFilter = ConvertFilterDTOToFilterEntity(Tracking_TrackingFilterDTO);
            TrackingFilter = await TrackingService.ToFilter(TrackingFilter);
            int count = await TrackingService.Count(TrackingFilter);
            return count;
        }

        [Route(TrackingRoute.List), HttpPost]
        public async Task<ActionResult<List<Tracking_TrackingDTO>>> List([FromBody] Tracking_TrackingFilterDTO Tracking_TrackingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TrackingFilter TrackingFilter = ConvertFilterDTOToFilterEntity(Tracking_TrackingFilterDTO);
            TrackingFilter = await TrackingService.ToFilter(TrackingFilter);
            List<Tracking> Trackings = await TrackingService.List(TrackingFilter);
            List<Tracking_TrackingDTO> Tracking_TrackingDTOs = Trackings
                .Select(c => new Tracking_TrackingDTO(c)).ToList();
            return Tracking_TrackingDTOs;
        }

        [Route(TrackingRoute.Get), HttpPost]
        public async Task<ActionResult<Tracking_TrackingDTO>> Get([FromBody]Tracking_TrackingDTO Tracking_TrackingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Tracking_TrackingDTO.Id))
                return Forbid();

            Tracking Tracking = await TrackingService.Get(Tracking_TrackingDTO.Id);
            return new Tracking_TrackingDTO(Tracking);
        }

        [Route(TrackingRoute.Create), HttpPost]
        public async Task<ActionResult<Tracking_TrackingDTO>> Create([FromBody] Tracking_TrackingDTO Tracking_TrackingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Tracking_TrackingDTO.Id))
                return Forbid();

            Tracking Tracking = ConvertDTOToEntity(Tracking_TrackingDTO);
            Tracking = await TrackingService.Create(Tracking);
            Tracking_TrackingDTO = new Tracking_TrackingDTO(Tracking);
            if (Tracking.IsValidated)
                return Tracking_TrackingDTO;
            else
                return BadRequest(Tracking_TrackingDTO);
        }

        [Route(TrackingRoute.Update), HttpPost]
        public async Task<ActionResult<Tracking_TrackingDTO>> Update([FromBody] Tracking_TrackingDTO Tracking_TrackingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Tracking_TrackingDTO.Id))
                return Forbid();

            Tracking Tracking = ConvertDTOToEntity(Tracking_TrackingDTO);
            Tracking = await TrackingService.Update(Tracking);
            Tracking_TrackingDTO = new Tracking_TrackingDTO(Tracking);
            if (Tracking.IsValidated)
                return Tracking_TrackingDTO;
            else
                return BadRequest(Tracking_TrackingDTO);
        }

        [Route(TrackingRoute.Delete), HttpPost]
        public async Task<ActionResult<Tracking_TrackingDTO>> Delete([FromBody] Tracking_TrackingDTO Tracking_TrackingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Tracking_TrackingDTO.Id))
                return Forbid();

            Tracking Tracking = ConvertDTOToEntity(Tracking_TrackingDTO);
            Tracking = await TrackingService.Delete(Tracking);
            Tracking_TrackingDTO = new Tracking_TrackingDTO(Tracking);
            if (Tracking.IsValidated)
                return Tracking_TrackingDTO;
            else
                return BadRequest(Tracking_TrackingDTO);
        }
        
        [Route(TrackingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TrackingFilter TrackingFilter = new TrackingFilter();
            TrackingFilter = await TrackingService.ToFilter(TrackingFilter);
            TrackingFilter.Id = new IdFilter { In = Ids };
            TrackingFilter.Selects = TrackingSelect.Id;
            TrackingFilter.Skip = 0;
            TrackingFilter.Take = int.MaxValue;

            List<Tracking> Trackings = await TrackingService.List(TrackingFilter);
            Trackings = await TrackingService.BulkDelete(Trackings);
            if (Trackings.Any(x => !x.IsValidated))
                return BadRequest(Trackings.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(TrackingRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            PlaceFilter PlaceFilter = new PlaceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PlaceSelect.ALL
            };
            List<Place> Places = await PlaceService.List(PlaceFilter);
            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = PlaceCheckingSelect.ALL
            };
            List<PlaceChecking> PlaceCheckings = await PlaceCheckingService.List(PlaceCheckingFilter);
            AppUserFilter TargetFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Targets = await AppUserService.List(TargetFilter);
            AppUserFilter TrackerFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Trackers = await AppUserService.List(TrackerFilter);
            List<Tracking> Trackings = new List<Tracking>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Trackings);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int TrackerIdColumn = 1 + StartColumn;
                int TargetIdColumn = 2 + StartColumn;
                int PlaceIdColumn = 3 + StartColumn;
                int PlaceCheckingIdColumn = 4 + StartColumn;
                int UsedColumn = 8 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string TrackerIdValue = worksheet.Cells[i + StartRow, TrackerIdColumn].Value?.ToString();
                    string TargetIdValue = worksheet.Cells[i + StartRow, TargetIdColumn].Value?.ToString();
                    string PlaceIdValue = worksheet.Cells[i + StartRow, PlaceIdColumn].Value?.ToString();
                    string PlaceCheckingIdValue = worksheet.Cells[i + StartRow, PlaceCheckingIdColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    Tracking Tracking = new Tracking();
                    Place Place = Places.Where(x => x.Id.ToString() == PlaceIdValue).FirstOrDefault();
                    Tracking.PlaceId = Place == null ? 0 : Place.Id;
                    Tracking.Place = Place;
                    PlaceChecking PlaceChecking = PlaceCheckings.Where(x => x.Id.ToString() == PlaceCheckingIdValue).FirstOrDefault();
                    Tracking.PlaceCheckingId = PlaceChecking == null ? 0 : PlaceChecking.Id;
                    Tracking.PlaceChecking = PlaceChecking;
                    AppUser Target = Targets.Where(x => x.Id.ToString() == TargetIdValue).FirstOrDefault();
                    Tracking.TargetId = Target == null ? 0 : Target.Id;
                    Tracking.Target = Target;
                    AppUser Tracker = Trackers.Where(x => x.Id.ToString() == TrackerIdValue).FirstOrDefault();
                    Tracking.TrackerId = Tracker == null ? 0 : Tracker.Id;
                    Tracking.Tracker = Tracker;
                    
                    Trackings.Add(Tracking);
                }
            }
            Trackings = await TrackingService.Import(Trackings);
            if (Trackings.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Trackings.Count; i++)
                {
                    Tracking Tracking = Trackings[i];
                    if (!Tracking.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Tracking.Errors.ContainsKey(nameof(Tracking.Id)))
                            Error += Tracking.Errors[nameof(Tracking.Id)];
                        if (Tracking.Errors.ContainsKey(nameof(Tracking.TrackerId)))
                            Error += Tracking.Errors[nameof(Tracking.TrackerId)];
                        if (Tracking.Errors.ContainsKey(nameof(Tracking.TargetId)))
                            Error += Tracking.Errors[nameof(Tracking.TargetId)];
                        if (Tracking.Errors.ContainsKey(nameof(Tracking.PlaceId)))
                            Error += Tracking.Errors[nameof(Tracking.PlaceId)];
                        if (Tracking.Errors.ContainsKey(nameof(Tracking.PlaceCheckingId)))
                            Error += Tracking.Errors[nameof(Tracking.PlaceCheckingId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(TrackingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Tracking_TrackingFilterDTO Tracking_TrackingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Tracking
                var TrackingFilter = ConvertFilterDTOToFilterEntity(Tracking_TrackingFilterDTO);
                TrackingFilter.Skip = 0;
                TrackingFilter.Take = int.MaxValue;
                TrackingFilter = await TrackingService.ToFilter(TrackingFilter);
                List<Tracking> Trackings = await TrackingService.List(TrackingFilter);

                var TrackingHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "TrackerId",
                        "TargetId",
                        "PlaceId",
                        "PlaceCheckingId",
                    }
                };
                List<object[]> TrackingData = new List<object[]>();
                for (int i = 0; i < Trackings.Count; i++)
                {
                    var Tracking = Trackings[i];
                    TrackingData.Add(new Object[]
                    {
                        Tracking.Id,
                        Tracking.TrackerId,
                        Tracking.TargetId,
                        Tracking.PlaceId,
                        Tracking.PlaceCheckingId,
                    });
                }
                excel.GenerateWorksheet("Tracking", TrackingHeaders, TrackingData);
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
                    });
                }
                excel.GenerateWorksheet("Place", PlaceHeaders, PlaceData);
                #endregion
                #region PlaceChecking
                var PlaceCheckingFilter = new PlaceCheckingFilter();
                PlaceCheckingFilter.Selects = PlaceCheckingSelect.ALL;
                PlaceCheckingFilter.OrderBy = PlaceCheckingOrder.Id;
                PlaceCheckingFilter.OrderType = OrderType.ASC;
                PlaceCheckingFilter.Skip = 0;
                PlaceCheckingFilter.Take = int.MaxValue;
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Tracking.xlsx");
        }

        [Route(TrackingRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Tracking_TrackingFilterDTO Tracking_TrackingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Tracking_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Tracking.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            TrackingFilter TrackingFilter = new TrackingFilter();
            TrackingFilter = await TrackingService.ToFilter(TrackingFilter);
            if (Id == 0)
            {

            }
            else
            {
                TrackingFilter.Id = new IdFilter { Equal = Id };
                int count = await TrackingService.Count(TrackingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Tracking ConvertDTOToEntity(Tracking_TrackingDTO Tracking_TrackingDTO)
        {
            Tracking Tracking = new Tracking();
            Tracking.Id = Tracking_TrackingDTO.Id;
            Tracking.TrackerId = Tracking_TrackingDTO.TrackerId;
            Tracking.TargetId = Tracking_TrackingDTO.TargetId;
            Tracking.PlaceId = Tracking_TrackingDTO.PlaceId;
            Tracking.PlaceCheckingId = Tracking_TrackingDTO.PlaceCheckingId;
            Tracking.Place = Tracking_TrackingDTO.Place == null ? null : new Place
            {
                Id = Tracking_TrackingDTO.Place.Id,
                Name = Tracking_TrackingDTO.Place.Name,
                PlaceGroupId = Tracking_TrackingDTO.Place.PlaceGroupId,
                Radius = Tracking_TrackingDTO.Place.Radius,
                Latitude = Tracking_TrackingDTO.Place.Latitude,
                Longtitude = Tracking_TrackingDTO.Place.Longtitude,
            };
            Tracking.PlaceChecking = Tracking_TrackingDTO.PlaceChecking == null ? null : new PlaceChecking
            {
                Id = Tracking_TrackingDTO.PlaceChecking.Id,
                AppUserId = Tracking_TrackingDTO.PlaceChecking.AppUserId,
                PlaceId = Tracking_TrackingDTO.PlaceChecking.PlaceId,
                PlaceCheckingStatusId = Tracking_TrackingDTO.PlaceChecking.PlaceCheckingStatusId,
                CheckInAt = Tracking_TrackingDTO.PlaceChecking.CheckInAt,
                CheckOutAt = Tracking_TrackingDTO.PlaceChecking.CheckOutAt,
            };
            Tracking.Target = Tracking_TrackingDTO.Target == null ? null : new AppUser
            {
                Id = Tracking_TrackingDTO.Target.Id,
                Username = Tracking_TrackingDTO.Target.Username,
                Password = Tracking_TrackingDTO.Target.Password,
                DisplayName = Tracking_TrackingDTO.Target.DisplayName,
                Email = Tracking_TrackingDTO.Target.Email,
                Phone = Tracking_TrackingDTO.Target.Phone,
            };
            Tracking.Tracker = Tracking_TrackingDTO.Tracker == null ? null : new AppUser
            {
                Id = Tracking_TrackingDTO.Tracker.Id,
                Username = Tracking_TrackingDTO.Tracker.Username,
                Password = Tracking_TrackingDTO.Tracker.Password,
                DisplayName = Tracking_TrackingDTO.Tracker.DisplayName,
                Email = Tracking_TrackingDTO.Tracker.Email,
                Phone = Tracking_TrackingDTO.Tracker.Phone,
            };
            Tracking.BaseLanguage = CurrentContext.Language;
            return Tracking;
        }

        private TrackingFilter ConvertFilterDTOToFilterEntity(Tracking_TrackingFilterDTO Tracking_TrackingFilterDTO)
        {
            TrackingFilter TrackingFilter = new TrackingFilter();
            TrackingFilter.Selects = TrackingSelect.ALL;
            TrackingFilter.Skip = Tracking_TrackingFilterDTO.Skip;
            TrackingFilter.Take = Tracking_TrackingFilterDTO.Take;
            TrackingFilter.OrderBy = Tracking_TrackingFilterDTO.OrderBy;
            TrackingFilter.OrderType = Tracking_TrackingFilterDTO.OrderType;

            TrackingFilter.Id = Tracking_TrackingFilterDTO.Id;
            TrackingFilter.TrackerId = Tracking_TrackingFilterDTO.TrackerId;
            TrackingFilter.TargetId = Tracking_TrackingFilterDTO.TargetId;
            TrackingFilter.PlaceId = Tracking_TrackingFilterDTO.PlaceId;
            TrackingFilter.PlaceCheckingId = Tracking_TrackingFilterDTO.PlaceCheckingId;
            TrackingFilter.CreatedAt = Tracking_TrackingFilterDTO.CreatedAt;
            TrackingFilter.UpdatedAt = Tracking_TrackingFilterDTO.UpdatedAt;
            return TrackingFilter;
        }
    }
}

