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

namespace LocatingApp.Services.MPlaceChecking
{
    public interface IPlaceCheckingService :  IServiceScoped
    {
        Task<int> Count(PlaceCheckingFilter PlaceCheckingFilter);
        Task<List<PlaceChecking>> List(PlaceCheckingFilter PlaceCheckingFilter);
        Task<PlaceChecking> Get(long Id);
        Task<PlaceChecking> Create(PlaceChecking PlaceChecking);
        Task<PlaceChecking> Update(PlaceChecking PlaceChecking);
        Task<PlaceChecking> Delete(PlaceChecking PlaceChecking);
        Task<List<PlaceChecking>> BulkDelete(List<PlaceChecking> PlaceCheckings);
        Task<List<PlaceChecking>> Import(List<PlaceChecking> PlaceCheckings);
        Task<PlaceCheckingFilter> ToFilter(PlaceCheckingFilter PlaceCheckingFilter);
    }

    public class PlaceCheckingService : BaseService, IPlaceCheckingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPlaceCheckingValidator PlaceCheckingValidator;

        public PlaceCheckingService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IPlaceCheckingValidator PlaceCheckingValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PlaceCheckingValidator = PlaceCheckingValidator;
        }
        public async Task<int> Count(PlaceCheckingFilter PlaceCheckingFilter)
        {
            try
            {
                int result = await UOW.PlaceCheckingRepository.Count(PlaceCheckingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return 0;
        }

        public async Task<List<PlaceChecking>> List(PlaceCheckingFilter PlaceCheckingFilter)
        {
            try
            {
                List<PlaceChecking> PlaceCheckings = await UOW.PlaceCheckingRepository.List(PlaceCheckingFilter);
                return PlaceCheckings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;
        }
        
        public async Task<PlaceChecking> Get(long Id)
        {
            PlaceChecking PlaceChecking = await UOW.PlaceCheckingRepository.Get(Id);
            if (PlaceChecking == null)
                return null;
            return PlaceChecking;
        }
        public async Task<PlaceChecking> Create(PlaceChecking PlaceChecking)
        {
            if (!await PlaceCheckingValidator.Create(PlaceChecking))
                return PlaceChecking;

            try
            {
                await UOW.PlaceCheckingRepository.Create(PlaceChecking);
                PlaceChecking = await UOW.PlaceCheckingRepository.Get(PlaceChecking.Id);
                await Logging.CreateAuditLog(PlaceChecking, new { }, nameof(PlaceCheckingService));
                return PlaceChecking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;
        }

        public async Task<PlaceChecking> Update(PlaceChecking PlaceChecking)
        {
            if (!await PlaceCheckingValidator.Update(PlaceChecking))
                return PlaceChecking;
            try
            {
                var oldData = await UOW.PlaceCheckingRepository.Get(PlaceChecking.Id);

                await UOW.PlaceCheckingRepository.Update(PlaceChecking);

                PlaceChecking = await UOW.PlaceCheckingRepository.Get(PlaceChecking.Id);
                await Logging.CreateAuditLog(PlaceChecking, oldData, nameof(PlaceCheckingService));
                return PlaceChecking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;
        }

        public async Task<PlaceChecking> Delete(PlaceChecking PlaceChecking)
        {
            if (!await PlaceCheckingValidator.Delete(PlaceChecking))
                return PlaceChecking;

            try
            {
                await UOW.PlaceCheckingRepository.Delete(PlaceChecking);
                await Logging.CreateAuditLog(new { }, PlaceChecking, nameof(PlaceCheckingService));
                return PlaceChecking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;
        }

        public async Task<List<PlaceChecking>> BulkDelete(List<PlaceChecking> PlaceCheckings)
        {
            if (!await PlaceCheckingValidator.BulkDelete(PlaceCheckings))
                return PlaceCheckings;

            try
            {
                await UOW.PlaceCheckingRepository.BulkDelete(PlaceCheckings);
                await Logging.CreateAuditLog(new { }, PlaceCheckings, nameof(PlaceCheckingService));
                return PlaceCheckings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;

        }
        
        public async Task<List<PlaceChecking>> Import(List<PlaceChecking> PlaceCheckings)
        {
            if (!await PlaceCheckingValidator.Import(PlaceCheckings))
                return PlaceCheckings;
            try
            {
                await UOW.PlaceCheckingRepository.BulkMerge(PlaceCheckings);

                await Logging.CreateAuditLog(PlaceCheckings, new { }, nameof(PlaceCheckingService));
                return PlaceCheckings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceCheckingService));
            }
            return null;
        }     
        
        public async Task<PlaceCheckingFilter> ToFilter(PlaceCheckingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PlaceCheckingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PlaceCheckingFilter subFilter = new PlaceCheckingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PlaceId))
                        subFilter.PlaceId = FilterBuilder.Merge(subFilter.PlaceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PlaceCheckingStatusId))
                        subFilter.PlaceCheckingStatusId = FilterBuilder.Merge(subFilter.PlaceCheckingStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CheckInAt))
                        subFilter.CheckInAt = FilterBuilder.Merge(subFilter.CheckInAt, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CheckOutAt))
                        subFilter.CheckOutAt = FilterBuilder.Merge(subFilter.CheckOutAt, FilterPermissionDefinition.DateFilter);
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
