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

namespace LocatingApp.Services.MAppUser
{
    public interface IAppUserService :  IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(List<AppUser> AppUsers);
        Task<AppUserFilter> ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return 0;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }
        
        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            return AppUser;
        }
        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Create(AppUser);
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);

                await UOW.AppUserRepository.Update(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(AppUser, oldData, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);
                await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                await Logging.CreateAuditLog(new { }, AppUsers, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;

        }
        
        public async Task<List<AppUser>> Import(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                await UOW.AppUserRepository.BulkMerge(AppUsers);

                await Logging.CreateAuditLog(AppUsers, new { }, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }     
        
        public async Task<AppUserFilter> ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = FilterBuilder.Merge(subFilter.Email, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = FilterBuilder.Merge(subFilter.Phone, FilterPermissionDefinition.StringFilter);
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
