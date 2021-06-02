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
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MPlaceGroup;

namespace LocatingApp.Rpc.place
{
    public partial class PlaceController : RpcController
    {
        private IPlaceGroupService PlaceGroupService;
        private IPlaceService PlaceService;
        private ICurrentContext CurrentContext;
        public PlaceController(
            IPlaceGroupService PlaceGroupService,
            IPlaceService PlaceService,
            ICurrentContext CurrentContext
        )
        {
            this.PlaceGroupService = PlaceGroupService;
            this.PlaceService = PlaceService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PlaceRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Place_PlaceFilterDTO Place_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceFilter PlaceFilter = ConvertFilterDTOToFilterEntity(Place_PlaceFilterDTO);
            PlaceFilter = await PlaceService.ToFilter(PlaceFilter);
            int count = await PlaceService.Count(PlaceFilter);
            return count;
        }

        [Route(PlaceRoute.List), HttpPost]
        public async Task<ActionResult<List<Place_PlaceDTO>>> List([FromBody] Place_PlaceFilterDTO Place_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceFilter PlaceFilter = ConvertFilterDTOToFilterEntity(Place_PlaceFilterDTO);
            PlaceFilter = await PlaceService.ToFilter(PlaceFilter);
            List<Place> Places = await PlaceService.List(PlaceFilter);
            List<Place_PlaceDTO> Place_PlaceDTOs = Places
                .Select(c => new Place_PlaceDTO(c)).ToList();
            return Place_PlaceDTOs;
        }

        [Route(PlaceRoute.Get), HttpPost]
        public async Task<ActionResult<Place_PlaceDTO>> Get([FromBody]Place_PlaceDTO Place_PlaceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Place_PlaceDTO.Id))
                return Forbid();

            Place Place = await PlaceService.Get(Place_PlaceDTO.Id);
            return new Place_PlaceDTO(Place);
        }

        [Route(PlaceRoute.Create), HttpPost]
        public async Task<ActionResult<Place_PlaceDTO>> Create([FromBody] Place_PlaceDTO Place_PlaceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Place_PlaceDTO.Id))
                return Forbid();

            Place Place = ConvertDTOToEntity(Place_PlaceDTO);
            Place = await PlaceService.Create(Place);
            Place_PlaceDTO = new Place_PlaceDTO(Place);
            if (Place.IsValidated)
                return Place_PlaceDTO;
            else
                return BadRequest(Place_PlaceDTO);
        }

        [Route(PlaceRoute.Update), HttpPost]
        public async Task<ActionResult<Place_PlaceDTO>> Update([FromBody] Place_PlaceDTO Place_PlaceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Place_PlaceDTO.Id))
                return Forbid();

            Place Place = ConvertDTOToEntity(Place_PlaceDTO);
            Place = await PlaceService.Update(Place);
            Place_PlaceDTO = new Place_PlaceDTO(Place);
            if (Place.IsValidated)
                return Place_PlaceDTO;
            else
                return BadRequest(Place_PlaceDTO);
        }

        [Route(PlaceRoute.Delete), HttpPost]
        public async Task<ActionResult<Place_PlaceDTO>> Delete([FromBody] Place_PlaceDTO Place_PlaceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Place_PlaceDTO.Id))
                return Forbid();

            Place Place = ConvertDTOToEntity(Place_PlaceDTO);
            Place = await PlaceService.Delete(Place);
            Place_PlaceDTO = new Place_PlaceDTO(Place);
            if (Place.IsValidated)
                return Place_PlaceDTO;
            else
                return BadRequest(Place_PlaceDTO);
        }
        
        [Route(PlaceRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceFilter PlaceFilter = new PlaceFilter();
            PlaceFilter = await PlaceService.ToFilter(PlaceFilter);
            PlaceFilter.Id = new IdFilter { In = Ids };
            PlaceFilter.Selects = PlaceSelect.Id;
            PlaceFilter.Skip = 0;
            PlaceFilter.Take = int.MaxValue;

            List<Place> Places = await PlaceService.List(PlaceFilter);
            Places = await PlaceService.BulkDelete(Places);
            if (Places.Any(x => !x.IsValidated))
                return BadRequest(Places.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(PlaceRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Place> Places = new List<Place>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Places);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PlaceGroupIdColumn = 2 + StartColumn;
                int RadiusColumn = 3 + StartColumn;
                int LatitudeColumn = 4 + StartColumn;
                int LongtitudeColumn = 5 + StartColumn;
                int UsedColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PlaceGroupIdValue = worksheet.Cells[i + StartRow, PlaceGroupIdColumn].Value?.ToString();
                    string RadiusValue = worksheet.Cells[i + StartRow, RadiusColumn].Value?.ToString();
                    string LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
                    string LongtitudeValue = worksheet.Cells[i + StartRow, LongtitudeColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    Place Place = new Place();
                    Place.Name = NameValue;
                    Place.Radius = long.TryParse(RadiusValue, out long Radius) ? Radius : 0;
                    Place.Latitude = decimal.TryParse(LatitudeValue, out decimal Latitude) ? Latitude : 0;
                    Place.Longtitude = decimal.TryParse(LongtitudeValue, out decimal Longtitude) ? Longtitude : 0;
                    
                    Places.Add(Place);
                }
            }
            Places = await PlaceService.Import(Places);
            if (Places.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Places.Count; i++)
                {
                    Place Place = Places[i];
                    if (!Place.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Place.Errors.ContainsKey(nameof(Place.Id)))
                            Error += Place.Errors[nameof(Place.Id)];
                        if (Place.Errors.ContainsKey(nameof(Place.Name)))
                            Error += Place.Errors[nameof(Place.Name)];
                        if (Place.Errors.ContainsKey(nameof(Place.PlaceGroupId)))
                            Error += Place.Errors[nameof(Place.PlaceGroupId)];
                        if (Place.Errors.ContainsKey(nameof(Place.Radius)))
                            Error += Place.Errors[nameof(Place.Radius)];
                        if (Place.Errors.ContainsKey(nameof(Place.Latitude)))
                            Error += Place.Errors[nameof(Place.Latitude)];
                        if (Place.Errors.ContainsKey(nameof(Place.Longtitude)))
                            Error += Place.Errors[nameof(Place.Longtitude)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(PlaceRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Place_PlaceFilterDTO Place_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Place
                var PlaceFilter = ConvertFilterDTOToFilterEntity(Place_PlaceFilterDTO);
                PlaceFilter.Skip = 0;
                PlaceFilter.Take = int.MaxValue;
                PlaceFilter = await PlaceService.ToFilter(PlaceFilter);
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
                
                #region PlaceGroup
                var PlaceGroupFilter = new PlaceGroupFilter();
                PlaceGroupFilter.Selects = PlaceGroupSelect.ALL;
                PlaceGroupFilter.OrderBy = PlaceGroupOrder.Id;
                PlaceGroupFilter.OrderType = OrderType.ASC;
                PlaceGroupFilter.Skip = 0;
                PlaceGroupFilter.Take = int.MaxValue;
                List<PlaceGroup> PlaceGroups = await PlaceGroupService.List(PlaceGroupFilter);

                var PlaceGroupHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ParentId",
                        "Name",
                        "Code",
                    }
                };
                List<object[]> PlaceGroupData = new List<object[]>();
                for (int i = 0; i < PlaceGroups.Count; i++)
                {
                    var PlaceGroup = PlaceGroups[i];
                    PlaceGroupData.Add(new Object[]
                    {
                        PlaceGroup.Id,
                        PlaceGroup.ParentId,
                        PlaceGroup.Name,
                        PlaceGroup.Code,
                    });
                }
                excel.GenerateWorksheet("PlaceGroup", PlaceGroupHeaders, PlaceGroupData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Place.xlsx");
        }

        [Route(PlaceRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Place_PlaceFilterDTO Place_PlaceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Place_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Place.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PlaceFilter PlaceFilter = new PlaceFilter();
            PlaceFilter = await PlaceService.ToFilter(PlaceFilter);
            if (Id == 0)
            {

            }
            else
            {
                PlaceFilter.Id = new IdFilter { Equal = Id };
                int count = await PlaceService.Count(PlaceFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Place ConvertDTOToEntity(Place_PlaceDTO Place_PlaceDTO)
        {
            Place Place = new Place();
            Place.Id = Place_PlaceDTO.Id;
            Place.Name = Place_PlaceDTO.Name;
            Place.PlaceGroupId = Place_PlaceDTO.PlaceGroupId;
            Place.Radius = Place_PlaceDTO.Radius;
            Place.Latitude = Place_PlaceDTO.Latitude;
            Place.Longtitude = Place_PlaceDTO.Longtitude;
            Place.PlaceGroup = Place_PlaceDTO.PlaceGroup == null ? null : new PlaceGroup
            {
                Id = Place_PlaceDTO.PlaceGroup.Id,
                ParentId = Place_PlaceDTO.PlaceGroup.ParentId,
                Name = Place_PlaceDTO.PlaceGroup.Name,
                Code = Place_PlaceDTO.PlaceGroup.Code,
            };
            Place.BaseLanguage = CurrentContext.Language;
            return Place;
        }

        private PlaceFilter ConvertFilterDTOToFilterEntity(Place_PlaceFilterDTO Place_PlaceFilterDTO)
        {
            PlaceFilter PlaceFilter = new PlaceFilter();
            PlaceFilter.Selects = PlaceSelect.ALL;
            PlaceFilter.Skip = Place_PlaceFilterDTO.Skip;
            PlaceFilter.Take = Place_PlaceFilterDTO.Take;
            PlaceFilter.OrderBy = Place_PlaceFilterDTO.OrderBy;
            PlaceFilter.OrderType = Place_PlaceFilterDTO.OrderType;

            PlaceFilter.Id = Place_PlaceFilterDTO.Id;
            PlaceFilter.Name = Place_PlaceFilterDTO.Name;
            PlaceFilter.PlaceGroupId = Place_PlaceFilterDTO.PlaceGroupId;
            PlaceFilter.Radius = Place_PlaceFilterDTO.Radius;
            PlaceFilter.Latitude = Place_PlaceFilterDTO.Latitude;
            PlaceFilter.Longtitude = Place_PlaceFilterDTO.Longtitude;
            PlaceFilter.CreatedAt = Place_PlaceFilterDTO.CreatedAt;
            PlaceFilter.UpdatedAt = Place_PlaceFilterDTO.UpdatedAt;
            return PlaceFilter;
        }
    }
}

