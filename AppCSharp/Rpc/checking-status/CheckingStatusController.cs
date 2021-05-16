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
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.checking_status
{
    public partial class CheckingStatusController : RpcController
    {
        private ICheckingStatusService CheckingStatusService;
        private ICurrentContext CurrentContext;
        public CheckingStatusController(
            ICheckingStatusService CheckingStatusService,
            ICurrentContext CurrentContext
        )
        {
            this.CheckingStatusService = CheckingStatusService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CheckingStatusRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] CheckingStatus_CheckingStatusFilterDTO CheckingStatus_CheckingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CheckingStatusFilter CheckingStatusFilter = ConvertFilterDTOToFilterEntity(CheckingStatus_CheckingStatusFilterDTO);
            CheckingStatusFilter = await CheckingStatusService.ToFilter(CheckingStatusFilter);
            int count = await CheckingStatusService.Count(CheckingStatusFilter);
            return count;
        }

        [Route(CheckingStatusRoute.List), HttpPost]
        public async Task<ActionResult<List<CheckingStatus_CheckingStatusDTO>>> List([FromBody] CheckingStatus_CheckingStatusFilterDTO CheckingStatus_CheckingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CheckingStatusFilter CheckingStatusFilter = ConvertFilterDTOToFilterEntity(CheckingStatus_CheckingStatusFilterDTO);
            CheckingStatusFilter = await CheckingStatusService.ToFilter(CheckingStatusFilter);
            List<CheckingStatus> CheckingStatuses = await CheckingStatusService.List(CheckingStatusFilter);
            List<CheckingStatus_CheckingStatusDTO> CheckingStatus_CheckingStatusDTOs = CheckingStatuses
                .Select(c => new CheckingStatus_CheckingStatusDTO(c)).ToList();
            return CheckingStatus_CheckingStatusDTOs;
        }

        [Route(CheckingStatusRoute.Get), HttpPost]
        public async Task<ActionResult<CheckingStatus_CheckingStatusDTO>> Get([FromBody]CheckingStatus_CheckingStatusDTO CheckingStatus_CheckingStatusDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CheckingStatus_CheckingStatusDTO.Id))
                return Forbid();

            CheckingStatus CheckingStatus = await CheckingStatusService.Get(CheckingStatus_CheckingStatusDTO.Id);
            return new CheckingStatus_CheckingStatusDTO(CheckingStatus);
        }

        [Route(CheckingStatusRoute.Create), HttpPost]
        public async Task<ActionResult<CheckingStatus_CheckingStatusDTO>> Create([FromBody] CheckingStatus_CheckingStatusDTO CheckingStatus_CheckingStatusDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(CheckingStatus_CheckingStatusDTO.Id))
                return Forbid();

            CheckingStatus CheckingStatus = ConvertDTOToEntity(CheckingStatus_CheckingStatusDTO);
            CheckingStatus = await CheckingStatusService.Create(CheckingStatus);
            CheckingStatus_CheckingStatusDTO = new CheckingStatus_CheckingStatusDTO(CheckingStatus);
            if (CheckingStatus.IsValidated)
                return CheckingStatus_CheckingStatusDTO;
            else
                return BadRequest(CheckingStatus_CheckingStatusDTO);
        }

        [Route(CheckingStatusRoute.Update), HttpPost]
        public async Task<ActionResult<CheckingStatus_CheckingStatusDTO>> Update([FromBody] CheckingStatus_CheckingStatusDTO CheckingStatus_CheckingStatusDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(CheckingStatus_CheckingStatusDTO.Id))
                return Forbid();

            CheckingStatus CheckingStatus = ConvertDTOToEntity(CheckingStatus_CheckingStatusDTO);
            CheckingStatus = await CheckingStatusService.Update(CheckingStatus);
            CheckingStatus_CheckingStatusDTO = new CheckingStatus_CheckingStatusDTO(CheckingStatus);
            if (CheckingStatus.IsValidated)
                return CheckingStatus_CheckingStatusDTO;
            else
                return BadRequest(CheckingStatus_CheckingStatusDTO);
        }

        [Route(CheckingStatusRoute.Delete), HttpPost]
        public async Task<ActionResult<CheckingStatus_CheckingStatusDTO>> Delete([FromBody] CheckingStatus_CheckingStatusDTO CheckingStatus_CheckingStatusDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(CheckingStatus_CheckingStatusDTO.Id))
                return Forbid();

            CheckingStatus CheckingStatus = ConvertDTOToEntity(CheckingStatus_CheckingStatusDTO);
            CheckingStatus = await CheckingStatusService.Delete(CheckingStatus);
            CheckingStatus_CheckingStatusDTO = new CheckingStatus_CheckingStatusDTO(CheckingStatus);
            if (CheckingStatus.IsValidated)
                return CheckingStatus_CheckingStatusDTO;
            else
                return BadRequest(CheckingStatus_CheckingStatusDTO);
        }
        
        [Route(CheckingStatusRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CheckingStatusFilter CheckingStatusFilter = new CheckingStatusFilter();
            CheckingStatusFilter = await CheckingStatusService.ToFilter(CheckingStatusFilter);
            CheckingStatusFilter.Id = new IdFilter { In = Ids };
            CheckingStatusFilter.Selects = CheckingStatusSelect.Id;
            CheckingStatusFilter.Skip = 0;
            CheckingStatusFilter.Take = int.MaxValue;

            List<CheckingStatus> CheckingStatuses = await CheckingStatusService.List(CheckingStatusFilter);
            CheckingStatuses = await CheckingStatusService.BulkDelete(CheckingStatuses);
            if (CheckingStatuses.Any(x => !x.IsValidated))
                return BadRequest(CheckingStatuses.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(CheckingStatusRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<CheckingStatus> CheckingStatuses = new List<CheckingStatus>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(CheckingStatuses);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    
                    CheckingStatus CheckingStatus = new CheckingStatus();
                    CheckingStatus.Code = CodeValue;
                    CheckingStatus.Name = NameValue;
                    
                    CheckingStatuses.Add(CheckingStatus);
                }
            }
            CheckingStatuses = await CheckingStatusService.Import(CheckingStatuses);
            if (CheckingStatuses.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < CheckingStatuses.Count; i++)
                {
                    CheckingStatus CheckingStatus = CheckingStatuses[i];
                    if (!CheckingStatus.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (CheckingStatus.Errors.ContainsKey(nameof(CheckingStatus.Id)))
                            Error += CheckingStatus.Errors[nameof(CheckingStatus.Id)];
                        if (CheckingStatus.Errors.ContainsKey(nameof(CheckingStatus.Code)))
                            Error += CheckingStatus.Errors[nameof(CheckingStatus.Code)];
                        if (CheckingStatus.Errors.ContainsKey(nameof(CheckingStatus.Name)))
                            Error += CheckingStatus.Errors[nameof(CheckingStatus.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CheckingStatusRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] CheckingStatus_CheckingStatusFilterDTO CheckingStatus_CheckingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region CheckingStatus
                var CheckingStatusFilter = ConvertFilterDTOToFilterEntity(CheckingStatus_CheckingStatusFilterDTO);
                CheckingStatusFilter.Skip = 0;
                CheckingStatusFilter.Take = int.MaxValue;
                CheckingStatusFilter = await CheckingStatusService.ToFilter(CheckingStatusFilter);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "CheckingStatus.xlsx");
        }

        [Route(CheckingStatusRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] CheckingStatus_CheckingStatusFilterDTO CheckingStatus_CheckingStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/CheckingStatus_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "CheckingStatus.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CheckingStatusFilter CheckingStatusFilter = new CheckingStatusFilter();
            CheckingStatusFilter = await CheckingStatusService.ToFilter(CheckingStatusFilter);
            if (Id == 0)
            {

            }
            else
            {
                CheckingStatusFilter.Id = new IdFilter { Equal = Id };
                int count = await CheckingStatusService.Count(CheckingStatusFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private CheckingStatus ConvertDTOToEntity(CheckingStatus_CheckingStatusDTO CheckingStatus_CheckingStatusDTO)
        {
            CheckingStatus CheckingStatus = new CheckingStatus();
            CheckingStatus.Id = CheckingStatus_CheckingStatusDTO.Id;
            CheckingStatus.Code = CheckingStatus_CheckingStatusDTO.Code;
            CheckingStatus.Name = CheckingStatus_CheckingStatusDTO.Name;
            CheckingStatus.BaseLanguage = CurrentContext.Language;
            return CheckingStatus;
        }

        private CheckingStatusFilter ConvertFilterDTOToFilterEntity(CheckingStatus_CheckingStatusFilterDTO CheckingStatus_CheckingStatusFilterDTO)
        {
            CheckingStatusFilter CheckingStatusFilter = new CheckingStatusFilter();
            CheckingStatusFilter.Selects = CheckingStatusSelect.ALL;
            CheckingStatusFilter.Skip = CheckingStatus_CheckingStatusFilterDTO.Skip;
            CheckingStatusFilter.Take = CheckingStatus_CheckingStatusFilterDTO.Take;
            CheckingStatusFilter.OrderBy = CheckingStatus_CheckingStatusFilterDTO.OrderBy;
            CheckingStatusFilter.OrderType = CheckingStatus_CheckingStatusFilterDTO.OrderType;

            CheckingStatusFilter.Id = CheckingStatus_CheckingStatusFilterDTO.Id;
            CheckingStatusFilter.Code = CheckingStatus_CheckingStatusFilterDTO.Code;
            CheckingStatusFilter.Name = CheckingStatus_CheckingStatusFilterDTO.Name;
            return CheckingStatusFilter;
        }
    }
}

