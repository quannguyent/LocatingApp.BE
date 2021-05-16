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

namespace LocatingApp.Services.MTracking
{
    public interface ITrackingService :  IServiceScoped
    {
        Task<int> Count(TrackingFilter TrackingFilter);
        Task<List<Tracking>> List(TrackingFilter TrackingFilter);
        Task<Tracking> Get(long Id);
        Task<Tracking> Create(Tracking Tracking);
        Task<Tracking> Update(Tracking Tracking);
        Task<Tracking> Delete(Tracking Tracking);
        Task<List<Tracking>> BulkDelete(List<Tracking> Trackings);
        Task<List<Tracking>> Import(List<Tracking> Trackings);
        Task<TrackingFilter> ToFilter(TrackingFilter TrackingFilter);
    }

    public class TrackingService : BaseService, ITrackingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITrackingValidator TrackingValidator;

        public TrackingService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ITrackingValidator TrackingValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TrackingValidator = TrackingValidator;
        }
        public async Task<int> Count(TrackingFilter TrackingFilter)
        {
            try
            {
                int result = await UOW.TrackingRepository.Count(TrackingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return 0;
        }

        public async Task<List<Tracking>> List(TrackingFilter TrackingFilter)
        {
            try
            {
                List<Tracking> Trackings = await UOW.TrackingRepository.List(TrackingFilter);
                return Trackings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;
        }
        
        public async Task<Tracking> Get(long Id)
        {
            Tracking Tracking = await UOW.TrackingRepository.Get(Id);
            if (Tracking == null)
                return null;
            return Tracking;
        }
        public async Task<Tracking> Create(Tracking Tracking)
        {
            if (!await TrackingValidator.Create(Tracking))
                return Tracking;

            try
            {
                await UOW.TrackingRepository.Create(Tracking);
                Tracking = await UOW.TrackingRepository.Get(Tracking.Id);
                await Logging.CreateAuditLog(Tracking, new { }, nameof(TrackingService));
                return Tracking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;
        }

        public async Task<Tracking> Update(Tracking Tracking)
        {
            if (!await TrackingValidator.Update(Tracking))
                return Tracking;
            try
            {
                var oldData = await UOW.TrackingRepository.Get(Tracking.Id);

                await UOW.TrackingRepository.Update(Tracking);

                Tracking = await UOW.TrackingRepository.Get(Tracking.Id);
                await Logging.CreateAuditLog(Tracking, oldData, nameof(TrackingService));
                return Tracking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;
        }

        public async Task<Tracking> Delete(Tracking Tracking)
        {
            if (!await TrackingValidator.Delete(Tracking))
                return Tracking;

            try
            {
                await UOW.TrackingRepository.Delete(Tracking);
                await Logging.CreateAuditLog(new { }, Tracking, nameof(TrackingService));
                return Tracking;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;
        }

        public async Task<List<Tracking>> BulkDelete(List<Tracking> Trackings)
        {
            if (!await TrackingValidator.BulkDelete(Trackings))
                return Trackings;

            try
            {
                await UOW.TrackingRepository.BulkDelete(Trackings);
                await Logging.CreateAuditLog(new { }, Trackings, nameof(TrackingService));
                return Trackings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;

        }
        
        public async Task<List<Tracking>> Import(List<Tracking> Trackings)
        {
            if (!await TrackingValidator.Import(Trackings))
                return Trackings;
            try
            {
                await UOW.TrackingRepository.BulkMerge(Trackings);

                await Logging.CreateAuditLog(Trackings, new { }, nameof(TrackingService));
                return Trackings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(TrackingService));
            }
            return null;
        }     
        
        public async Task<TrackingFilter> ToFilter(TrackingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<TrackingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                TrackingFilter subFilter = new TrackingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TrackerId))
                        subFilter.TrackerId = FilterBuilder.Merge(subFilter.TrackerId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TargetId))
                        subFilter.TargetId = FilterBuilder.Merge(subFilter.TargetId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PlaceId))
                        subFilter.PlaceId = FilterBuilder.Merge(subFilter.PlaceId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PlaceCheckingId))
                        subFilter.PlaceCheckingId = FilterBuilder.Merge(subFilter.PlaceCheckingId, FilterPermissionDefinition.IdFilter);
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
