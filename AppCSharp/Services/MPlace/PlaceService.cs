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

namespace LocatingApp.Services.MPlace
{
    public interface IPlaceService :  IServiceScoped
    {
        Task<int> Count(PlaceFilter PlaceFilter);
        Task<List<Place>> List(PlaceFilter PlaceFilter);
        Task<Place> Get(long Id);
        Task<Place> Create(Place Place);
        Task<Place> Update(Place Place);
        Task<Place> Delete(Place Place);
        Task<List<Place>> BulkDelete(List<Place> Places);
        Task<List<Place>> Import(List<Place> Places);
        Task<PlaceFilter> ToFilter(PlaceFilter PlaceFilter);
    }

    public class PlaceService : BaseService, IPlaceService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPlaceValidator PlaceValidator;

        public PlaceService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IPlaceValidator PlaceValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PlaceValidator = PlaceValidator;
        }
        public async Task<int> Count(PlaceFilter PlaceFilter)
        {
            try
            {
                int result = await UOW.PlaceRepository.Count(PlaceFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return 0;
        }

        public async Task<List<Place>> List(PlaceFilter PlaceFilter)
        {
            try
            {
                List<Place> Places = await UOW.PlaceRepository.List(PlaceFilter);
                return Places;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;
        }
        
        public async Task<Place> Get(long Id)
        {
            Place Place = await UOW.PlaceRepository.Get(Id);
            if (Place == null)
                return null;
            return Place;
        }
        public async Task<Place> Create(Place Place)
        {
            if (!await PlaceValidator.Create(Place))
                return Place;

            try
            {
                await UOW.PlaceRepository.Create(Place);
                Place = await UOW.PlaceRepository.Get(Place.Id);
                await Logging.CreateAuditLog(Place, new { }, nameof(PlaceService));
                return Place;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;
        }

        public async Task<Place> Update(Place Place)
        {
            if (!await PlaceValidator.Update(Place))
                return Place;
            try
            {
                var oldData = await UOW.PlaceRepository.Get(Place.Id);

                await UOW.PlaceRepository.Update(Place);

                Place = await UOW.PlaceRepository.Get(Place.Id);
                await Logging.CreateAuditLog(Place, oldData, nameof(PlaceService));
                return Place;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;
        }

        public async Task<Place> Delete(Place Place)
        {
            if (!await PlaceValidator.Delete(Place))
                return Place;

            try
            {
                await UOW.PlaceRepository.Delete(Place);
                await Logging.CreateAuditLog(new { }, Place, nameof(PlaceService));
                return Place;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;
        }

        public async Task<List<Place>> BulkDelete(List<Place> Places)
        {
            if (!await PlaceValidator.BulkDelete(Places))
                return Places;

            try
            {
                await UOW.PlaceRepository.BulkDelete(Places);
                await Logging.CreateAuditLog(new { }, Places, nameof(PlaceService));
                return Places;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;

        }
        
        public async Task<List<Place>> Import(List<Place> Places)
        {
            if (!await PlaceValidator.Import(Places))
                return Places;
            try
            {
                await UOW.PlaceRepository.BulkMerge(Places);

                await Logging.CreateAuditLog(Places, new { }, nameof(PlaceService));
                return Places;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceService));
            }
            return null;
        }     
        
        public async Task<PlaceFilter> ToFilter(PlaceFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PlaceFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PlaceFilter subFilter = new PlaceFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.PlaceGroupId))
                        subFilter.PlaceGroupId = FilterBuilder.Merge(subFilter.PlaceGroupId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Radius))
                        subFilter.Radius = FilterBuilder.Merge(subFilter.Radius, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Latitude))
                        subFilter.Latitude = FilterBuilder.Merge(subFilter.Latitude, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Longtitude))
                        subFilter.Longtitude = FilterBuilder.Merge(subFilter.Longtitude, FilterPermissionDefinition.DecimalFilter);
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
