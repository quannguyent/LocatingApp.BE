using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MCheckingStatus
{
    public interface ICheckingStatusValidator : IServiceScoped
    {
        Task<bool> Create(CheckingStatus CheckingStatus);
        Task<bool> Update(CheckingStatus CheckingStatus);
        Task<bool> Delete(CheckingStatus CheckingStatus);
        Task<bool> BulkDelete(List<CheckingStatus> CheckingStatuses);
        Task<bool> Import(List<CheckingStatus> CheckingStatuses);
    }

    public class CheckingStatusValidator : ICheckingStatusValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public CheckingStatusValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(CheckingStatus CheckingStatus)
        {
            CheckingStatusFilter CheckingStatusFilter = new CheckingStatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = CheckingStatus.Id },
                Selects = CheckingStatusSelect.Id
            };

            int count = await UOW.CheckingStatusRepository.Count(CheckingStatusFilter);
            if (count == 0)
                CheckingStatus.AddError(nameof(CheckingStatusValidator), nameof(CheckingStatus.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(CheckingStatus CheckingStatus)
        {
            return CheckingStatus.IsValidated;
        }

        public async Task<bool> Update(CheckingStatus CheckingStatus)
        {
            if (await ValidateId(CheckingStatus))
            {
            }
            return CheckingStatus.IsValidated;
        }

        public async Task<bool> Delete(CheckingStatus CheckingStatus)
        {
            if (await ValidateId(CheckingStatus))
            {
            }
            return CheckingStatus.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<CheckingStatus> CheckingStatuses)
        {
            foreach (CheckingStatus CheckingStatus in CheckingStatuses)
            {
                await Delete(CheckingStatus);
            }
            return CheckingStatuses.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<CheckingStatus> CheckingStatuses)
        {
            return true;
        }
    }
}
