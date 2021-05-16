using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
    }

    public class AppUserValidator : IAppUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(AppUser AppUser)
        {
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
            }
            return AppUser.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            foreach (AppUser AppUser in AppUsers)
            {
                await Delete(AppUser);
            }
            return AppUsers.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            return true;
        }
    }
}
