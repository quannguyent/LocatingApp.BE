using LocatingApp.Common;
using LocatingApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using LocatingApp.Repositories;
using LocatingApp.Entities;
using LocatingApp.Enums;

namespace LocatingApp.Services.MCheckingStatus
{
    public interface ICheckingStatusService :  IServiceScoped
    {
        Task<int> Count(CheckingStatusFilter CheckingStatusFilter);
        Task<List<CheckingStatus>> List(CheckingStatusFilter CheckingStatusFilter);
        Task<CheckingStatus> Get(long Id);
        Task<CheckingStatus> Create(CheckingStatus CheckingStatus);
        Task<CheckingStatus> Update(CheckingStatus CheckingStatus);
        Task<CheckingStatus> Delete(CheckingStatus CheckingStatus);
        Task<List<CheckingStatus>> BulkDelete(List<CheckingStatus> CheckingStatuses);
        Task<List<CheckingStatus>> Import(List<CheckingStatus> CheckingStatuses);
        Task<CheckingStatusFilter> ToFilter(CheckingStatusFilter CheckingStatusFilter);
    }

    public class CheckingStatusService : BaseService, ICheckingStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICheckingStatusValidator CheckingStatusValidator;

        public CheckingStatusService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ICheckingStatusValidator CheckingStatusValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CheckingStatusValidator = CheckingStatusValidator;
        }
        public async Task<int> Count(CheckingStatusFilter CheckingStatusFilter)
        {
            try
            {
                int result = await UOW.CheckingStatusRepository.Count(CheckingStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return 0;
        }

        public async Task<List<CheckingStatus>> List(CheckingStatusFilter CheckingStatusFilter)
        {
            try
            {
                List<CheckingStatus> CheckingStatuses = await UOW.CheckingStatusRepository.List(CheckingStatusFilter);
                return CheckingStatuses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;
        }
        
        public async Task<CheckingStatus> Get(long Id)
        {
            CheckingStatus CheckingStatus = await UOW.CheckingStatusRepository.Get(Id);
            if (CheckingStatus == null)
                return null;
            return CheckingStatus;
        }
        public async Task<CheckingStatus> Create(CheckingStatus CheckingStatus)
        {
            if (!await CheckingStatusValidator.Create(CheckingStatus))
                return CheckingStatus;

            try
            {
                await UOW.CheckingStatusRepository.Create(CheckingStatus);
                CheckingStatus = await UOW.CheckingStatusRepository.Get(CheckingStatus.Id);
                await Logging.CreateAuditLog(CheckingStatus, new { }, nameof(CheckingStatusService));
                return CheckingStatus;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;
        }

        public async Task<CheckingStatus> Update(CheckingStatus CheckingStatus)
        {
            if (!await CheckingStatusValidator.Update(CheckingStatus))
                return CheckingStatus;
            try
            {
                var oldData = await UOW.CheckingStatusRepository.Get(CheckingStatus.Id);

                await UOW.CheckingStatusRepository.Update(CheckingStatus);

                CheckingStatus = await UOW.CheckingStatusRepository.Get(CheckingStatus.Id);
                await Logging.CreateAuditLog(CheckingStatus, oldData, nameof(CheckingStatusService));
                return CheckingStatus;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;
        }

        public async Task<CheckingStatus> Delete(CheckingStatus CheckingStatus)
        {
            if (!await CheckingStatusValidator.Delete(CheckingStatus))
                return CheckingStatus;

            try
            {
                await UOW.CheckingStatusRepository.Delete(CheckingStatus);
                await Logging.CreateAuditLog(new { }, CheckingStatus, nameof(CheckingStatusService));
                return CheckingStatus;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;
        }

        public async Task<List<CheckingStatus>> BulkDelete(List<CheckingStatus> CheckingStatuses)
        {
            if (!await CheckingStatusValidator.BulkDelete(CheckingStatuses))
                return CheckingStatuses;

            try
            {
                await UOW.CheckingStatusRepository.BulkDelete(CheckingStatuses);
                await Logging.CreateAuditLog(new { }, CheckingStatuses, nameof(CheckingStatusService));
                return CheckingStatuses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;

        }
        
        public async Task<List<CheckingStatus>> Import(List<CheckingStatus> CheckingStatuses)
        {
            if (!await CheckingStatusValidator.Import(CheckingStatuses))
                return CheckingStatuses;
            try
            {
                await UOW.CheckingStatusRepository.BulkMerge(CheckingStatuses);

                await Logging.CreateAuditLog(CheckingStatuses, new { }, nameof(CheckingStatusService));
                return CheckingStatuses;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(CheckingStatusService));
            }
            return null;
        }     
        
        public async Task<CheckingStatusFilter> ToFilter(CheckingStatusFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CheckingStatusFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CheckingStatusFilter subFilter = new CheckingStatusFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
