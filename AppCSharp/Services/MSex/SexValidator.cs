using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;
using RestSharp.Validation;

namespace LocatingApp.Services.MSex
{
    public interface ISexValidator : IServiceScoped
    {
        Task<bool> Create(Sex Sex);
        Task<bool> Update(Sex Sex);
        Task<bool> Delete(Sex Sex);
        Task<bool> BulkDelete(List<Sex> Sexes);
        Task<bool> Import(List<Sex> Sexes);
    }

    public class SexValidator : ISexValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            NameEmpty,
            NameExisted,
            CodeEmpty,
            CodeExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public SexValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Sex Sex)
        {
            SexFilter SexFilter = new SexFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Sex.Id },
                Selects = SexSelect.Id
            };

            int count = await UOW.SexRepository.Count(SexFilter);
            if (count == 0)
                Sex.AddError(nameof(SexValidator), nameof(Sex.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        private async Task<bool> ValidateCode(Sex Sex)
        {
            if (string.IsNullOrEmpty(Sex.Code))
            {
                Sex.AddError(nameof(SexValidator), nameof(Sex.Code), ErrorCode.CodeEmpty);
                return false;
            }
            SexFilter SexFilter = new SexFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Sex.Id },
                Code = new StringFilter { Equal = Sex.Code },
                Selects = SexSelect.Code
            };

            int count = await UOW.SexRepository.Count(SexFilter);
            if (count != 0)
                Sex.AddError(nameof(SexValidator), nameof(Sex.Code), ErrorCode.CodeExisted);
            return count == 0;
        }

        private async Task<bool> ValidateName(Sex Sex)
        {
            if (string.IsNullOrEmpty(Sex.Name))
            {
                Sex.AddError(nameof(SexValidator), nameof(Sex.Name), ErrorCode.NameEmpty);
                return false;
            }
            return true;
        }
        public async Task<bool>Create(Sex Sex)
        {
            await ValidateCode(Sex);
            await ValidateName(Sex);
            return Sex.IsValidated;
        }

        public async Task<bool> Update(Sex Sex)
        {
            if (await ValidateId(Sex))
            {
                await ValidateCode(Sex);
                await ValidateName(Sex);
            }
            return Sex.IsValidated;
        }

        public async Task<bool> Delete(Sex Sex)
        {
            if (await ValidateId(Sex))
            {
            }
            return Sex.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Sex> Sexes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Sex> Sexes)
        {
            return true;
        }
    }
}
