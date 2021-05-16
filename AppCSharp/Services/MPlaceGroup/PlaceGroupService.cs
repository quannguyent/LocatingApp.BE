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

namespace LocatingApp.Services.MPlaceGroup
{
    public interface IPlaceGroupService :  IServiceScoped
    {
        Task<int> Count(PlaceGroupFilter PlaceGroupFilter);
        Task<List<PlaceGroup>> List(PlaceGroupFilter PlaceGroupFilter);
        Task<PlaceGroup> Get(long Id);
        Task<PlaceGroup> Create(PlaceGroup PlaceGroup);
        Task<PlaceGroup> Update(PlaceGroup PlaceGroup);
        Task<PlaceGroup> Delete(PlaceGroup PlaceGroup);
        Task<List<PlaceGroup>> BulkDelete(List<PlaceGroup> PlaceGroups);
        Task<List<PlaceGroup>> Import(List<PlaceGroup> PlaceGroups);
        Task<PlaceGroupFilter> ToFilter(PlaceGroupFilter PlaceGroupFilter);
    }

    public class PlaceGroupService : BaseService, IPlaceGroupService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IPlaceGroupValidator PlaceGroupValidator;

        public PlaceGroupService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IPlaceGroupValidator PlaceGroupValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.PlaceGroupValidator = PlaceGroupValidator;
        }
        public async Task<int> Count(PlaceGroupFilter PlaceGroupFilter)
        {
            try
            {
                int result = await UOW.PlaceGroupRepository.Count(PlaceGroupFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return 0;
        }

        public async Task<List<PlaceGroup>> List(PlaceGroupFilter PlaceGroupFilter)
        {
            try
            {
                List<PlaceGroup> PlaceGroups = await UOW.PlaceGroupRepository.List(PlaceGroupFilter);
                return PlaceGroups;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;
        }
        
        public async Task<PlaceGroup> Get(long Id)
        {
            PlaceGroup PlaceGroup = await UOW.PlaceGroupRepository.Get(Id);
            if (PlaceGroup == null)
                return null;
            return PlaceGroup;
        }
        public async Task<PlaceGroup> Create(PlaceGroup PlaceGroup)
        {
            if (!await PlaceGroupValidator.Create(PlaceGroup))
                return PlaceGroup;

            try
            {
                await UOW.PlaceGroupRepository.Create(PlaceGroup);
                PlaceGroup = await UOW.PlaceGroupRepository.Get(PlaceGroup.Id);
                await Logging.CreateAuditLog(PlaceGroup, new { }, nameof(PlaceGroupService));
                return PlaceGroup;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;
        }

        public async Task<PlaceGroup> Update(PlaceGroup PlaceGroup)
        {
            if (!await PlaceGroupValidator.Update(PlaceGroup))
                return PlaceGroup;
            try
            {
                var oldData = await UOW.PlaceGroupRepository.Get(PlaceGroup.Id);

                await UOW.PlaceGroupRepository.Update(PlaceGroup);

                PlaceGroup = await UOW.PlaceGroupRepository.Get(PlaceGroup.Id);
                await Logging.CreateAuditLog(PlaceGroup, oldData, nameof(PlaceGroupService));
                return PlaceGroup;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;
        }

        public async Task<PlaceGroup> Delete(PlaceGroup PlaceGroup)
        {
            if (!await PlaceGroupValidator.Delete(PlaceGroup))
                return PlaceGroup;

            try
            {
                await UOW.PlaceGroupRepository.Delete(PlaceGroup);
                await Logging.CreateAuditLog(new { }, PlaceGroup, nameof(PlaceGroupService));
                return PlaceGroup;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;
        }

        public async Task<List<PlaceGroup>> BulkDelete(List<PlaceGroup> PlaceGroups)
        {
            if (!await PlaceGroupValidator.BulkDelete(PlaceGroups))
                return PlaceGroups;

            try
            {
                await UOW.PlaceGroupRepository.BulkDelete(PlaceGroups);
                await Logging.CreateAuditLog(new { }, PlaceGroups, nameof(PlaceGroupService));
                return PlaceGroups;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;

        }
        
        public async Task<List<PlaceGroup>> Import(List<PlaceGroup> PlaceGroups)
        {
            if (!await PlaceGroupValidator.Import(PlaceGroups))
                return PlaceGroups;
            try
            {
                await UOW.PlaceGroupRepository.BulkMerge(PlaceGroups);

                await Logging.CreateAuditLog(PlaceGroups, new { }, nameof(PlaceGroupService));
                return PlaceGroups;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(PlaceGroupService));
            }
            return null;
        }     
        
        public async Task<PlaceGroupFilter> ToFilter(PlaceGroupFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<PlaceGroupFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                PlaceGroupFilter subFilter = new PlaceGroupFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = FilterBuilder.Merge(subFilter.ParentId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
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
