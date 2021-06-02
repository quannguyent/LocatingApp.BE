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
using LocatingApp.Services.MPlaceGroup;

namespace LocatingApp.Rpc.place_group
{
    public partial class PlaceGroupController : RpcController
    {
        private IPlaceGroupService PlaceGroupService;
        private ICurrentContext CurrentContext;
        public PlaceGroupController(
            IPlaceGroupService PlaceGroupService,
            ICurrentContext CurrentContext
        )
        {
            this.PlaceGroupService = PlaceGroupService;
            this.CurrentContext = CurrentContext;
        }

        [Route(PlaceGroupRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceGroupFilter PlaceGroupFilter = ConvertFilterDTOToFilterEntity(PlaceGroup_PlaceGroupFilterDTO);
            PlaceGroupFilter = await PlaceGroupService.ToFilter(PlaceGroupFilter);
            int count = await PlaceGroupService.Count(PlaceGroupFilter);
            return count;
        }

        [Route(PlaceGroupRoute.List), HttpPost]
        public async Task<ActionResult<List<PlaceGroup_PlaceGroupDTO>>> List([FromBody] PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceGroupFilter PlaceGroupFilter = ConvertFilterDTOToFilterEntity(PlaceGroup_PlaceGroupFilterDTO);
            PlaceGroupFilter = await PlaceGroupService.ToFilter(PlaceGroupFilter);
            List<PlaceGroup> PlaceGroups = await PlaceGroupService.List(PlaceGroupFilter);
            List<PlaceGroup_PlaceGroupDTO> PlaceGroup_PlaceGroupDTOs = PlaceGroups
                .Select(c => new PlaceGroup_PlaceGroupDTO(c)).ToList();
            return PlaceGroup_PlaceGroupDTOs;
        }

        [Route(PlaceGroupRoute.Get), HttpPost]
        public async Task<ActionResult<PlaceGroup_PlaceGroupDTO>> Get([FromBody]PlaceGroup_PlaceGroupDTO PlaceGroup_PlaceGroupDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PlaceGroup_PlaceGroupDTO.Id))
                return Forbid();

            PlaceGroup PlaceGroup = await PlaceGroupService.Get(PlaceGroup_PlaceGroupDTO.Id);
            return new PlaceGroup_PlaceGroupDTO(PlaceGroup);
        }

        [Route(PlaceGroupRoute.Create), HttpPost]
        public async Task<ActionResult<PlaceGroup_PlaceGroupDTO>> Create([FromBody] PlaceGroup_PlaceGroupDTO PlaceGroup_PlaceGroupDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PlaceGroup_PlaceGroupDTO.Id))
                return Forbid();

            PlaceGroup PlaceGroup = ConvertDTOToEntity(PlaceGroup_PlaceGroupDTO);
            PlaceGroup = await PlaceGroupService.Create(PlaceGroup);
            PlaceGroup_PlaceGroupDTO = new PlaceGroup_PlaceGroupDTO(PlaceGroup);
            if (PlaceGroup.IsValidated)
                return PlaceGroup_PlaceGroupDTO;
            else
                return BadRequest(PlaceGroup_PlaceGroupDTO);
        }

        [Route(PlaceGroupRoute.Update), HttpPost]
        public async Task<ActionResult<PlaceGroup_PlaceGroupDTO>> Update([FromBody] PlaceGroup_PlaceGroupDTO PlaceGroup_PlaceGroupDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(PlaceGroup_PlaceGroupDTO.Id))
                return Forbid();

            PlaceGroup PlaceGroup = ConvertDTOToEntity(PlaceGroup_PlaceGroupDTO);
            PlaceGroup = await PlaceGroupService.Update(PlaceGroup);
            PlaceGroup_PlaceGroupDTO = new PlaceGroup_PlaceGroupDTO(PlaceGroup);
            if (PlaceGroup.IsValidated)
                return PlaceGroup_PlaceGroupDTO;
            else
                return BadRequest(PlaceGroup_PlaceGroupDTO);
        }

        [Route(PlaceGroupRoute.Delete), HttpPost]
        public async Task<ActionResult<PlaceGroup_PlaceGroupDTO>> Delete([FromBody] PlaceGroup_PlaceGroupDTO PlaceGroup_PlaceGroupDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(PlaceGroup_PlaceGroupDTO.Id))
                return Forbid();

            PlaceGroup PlaceGroup = ConvertDTOToEntity(PlaceGroup_PlaceGroupDTO);
            PlaceGroup = await PlaceGroupService.Delete(PlaceGroup);
            PlaceGroup_PlaceGroupDTO = new PlaceGroup_PlaceGroupDTO(PlaceGroup);
            if (PlaceGroup.IsValidated)
                return PlaceGroup_PlaceGroupDTO;
            else
                return BadRequest(PlaceGroup_PlaceGroupDTO);
        }
        
        [Route(PlaceGroupRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter();
            PlaceGroupFilter = await PlaceGroupService.ToFilter(PlaceGroupFilter);
            PlaceGroupFilter.Id = new IdFilter { In = Ids };
            PlaceGroupFilter.Selects = PlaceGroupSelect.Id;
            PlaceGroupFilter.Skip = 0;
            PlaceGroupFilter.Take = int.MaxValue;

            List<PlaceGroup> PlaceGroups = await PlaceGroupService.List(PlaceGroupFilter);
            PlaceGroups = await PlaceGroupService.BulkDelete(PlaceGroups);
            if (PlaceGroups.Any(x => !x.IsValidated))
                return BadRequest(PlaceGroups.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(PlaceGroupRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<PlaceGroup> PlaceGroups = new List<PlaceGroup>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(PlaceGroups);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ParentIdColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int CodeColumn = 3 + StartColumn;
                int UsedColumn = 7 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string ParentIdValue = worksheet.Cells[i + StartRow, ParentIdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string UsedValue = worksheet.Cells[i + StartRow, UsedColumn].Value?.ToString();
                    
                    PlaceGroup PlaceGroup = new PlaceGroup();
                    PlaceGroup.Name = NameValue;
                    PlaceGroup.Code = CodeValue;
                    
                    PlaceGroups.Add(PlaceGroup);
                }
            }
            PlaceGroups = await PlaceGroupService.Import(PlaceGroups);
            if (PlaceGroups.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < PlaceGroups.Count; i++)
                {
                    PlaceGroup PlaceGroup = PlaceGroups[i];
                    if (!PlaceGroup.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (PlaceGroup.Errors.ContainsKey(nameof(PlaceGroup.Id)))
                            Error += PlaceGroup.Errors[nameof(PlaceGroup.Id)];
                        if (PlaceGroup.Errors.ContainsKey(nameof(PlaceGroup.ParentId)))
                            Error += PlaceGroup.Errors[nameof(PlaceGroup.ParentId)];
                        if (PlaceGroup.Errors.ContainsKey(nameof(PlaceGroup.Name)))
                            Error += PlaceGroup.Errors[nameof(PlaceGroup.Name)];
                        if (PlaceGroup.Errors.ContainsKey(nameof(PlaceGroup.Code)))
                            Error += PlaceGroup.Errors[nameof(PlaceGroup.Code)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(PlaceGroupRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region PlaceGroup
                var PlaceGroupFilter = ConvertFilterDTOToFilterEntity(PlaceGroup_PlaceGroupFilterDTO);
                PlaceGroupFilter.Skip = 0;
                PlaceGroupFilter.Take = int.MaxValue;
                PlaceGroupFilter = await PlaceGroupService.ToFilter(PlaceGroupFilter);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "PlaceGroup.xlsx");
        }

        [Route(PlaceGroupRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/PlaceGroup_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "PlaceGroup.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter();
            PlaceGroupFilter = await PlaceGroupService.ToFilter(PlaceGroupFilter);
            if (Id == 0)
            {

            }
            else
            {
                PlaceGroupFilter.Id = new IdFilter { Equal = Id };
                int count = await PlaceGroupService.Count(PlaceGroupFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private PlaceGroup ConvertDTOToEntity(PlaceGroup_PlaceGroupDTO PlaceGroup_PlaceGroupDTO)
        {
            PlaceGroup PlaceGroup = new PlaceGroup();
            PlaceGroup.Id = PlaceGroup_PlaceGroupDTO.Id;
            PlaceGroup.ParentId = PlaceGroup_PlaceGroupDTO.ParentId;
            PlaceGroup.Name = PlaceGroup_PlaceGroupDTO.Name;
            PlaceGroup.Code = PlaceGroup_PlaceGroupDTO.Code;
            PlaceGroup.Parent = PlaceGroup_PlaceGroupDTO.Parent == null ? null : new PlaceGroup
            {
                Id = PlaceGroup_PlaceGroupDTO.Parent.Id,
                ParentId = PlaceGroup_PlaceGroupDTO.Parent.ParentId,
                Name = PlaceGroup_PlaceGroupDTO.Parent.Name,
                Code = PlaceGroup_PlaceGroupDTO.Parent.Code,
            };
            PlaceGroup.BaseLanguage = CurrentContext.Language;
            return PlaceGroup;
        }

        private PlaceGroupFilter ConvertFilterDTOToFilterEntity(PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter();
            PlaceGroupFilter.Selects = PlaceGroupSelect.ALL;
            PlaceGroupFilter.Skip = 0;
            PlaceGroupFilter.Take = 99999;
            PlaceGroupFilter.OrderBy = PlaceGroup_PlaceGroupFilterDTO.OrderBy;
            PlaceGroupFilter.OrderType = PlaceGroup_PlaceGroupFilterDTO.OrderType;

            PlaceGroupFilter.Id = PlaceGroup_PlaceGroupFilterDTO.Id;
            PlaceGroupFilter.ParentId = PlaceGroup_PlaceGroupFilterDTO.ParentId;
            PlaceGroupFilter.Name = PlaceGroup_PlaceGroupFilterDTO.Name;
            PlaceGroupFilter.Code = PlaceGroup_PlaceGroupFilterDTO.Code;
            PlaceGroupFilter.CreatedAt = PlaceGroup_PlaceGroupFilterDTO.CreatedAt;
            PlaceGroupFilter.UpdatedAt = PlaceGroup_PlaceGroupFilterDTO.UpdatedAt;
            return PlaceGroupFilter;
        }
    }
}

