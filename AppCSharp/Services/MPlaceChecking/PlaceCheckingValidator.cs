using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MPlaceChecking
{
    public interface IPlaceCheckingValidator : IServiceScoped
    {
        Task<bool> Create(PlaceChecking PlaceChecking);
        Task<bool> Update(PlaceChecking PlaceChecking);
        Task<bool> Delete(PlaceChecking PlaceChecking);
        Task<bool> BulkDelete(List<PlaceChecking> PlaceCheckings);
        Task<bool> Import(List<PlaceChecking> PlaceCheckings);
    }

    public class PlaceCheckingValidator : IPlaceCheckingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PlaceCheckingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PlaceChecking PlaceChecking)
        {
            PlaceCheckingFilter PlaceCheckingFilter = new PlaceCheckingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PlaceChecking.Id },
                Selects = PlaceCheckingSelect.Id
            };

            int count = await UOW.PlaceCheckingRepository.Count(PlaceCheckingFilter);
            if (count == 0)
                PlaceChecking.AddError(nameof(PlaceCheckingValidator), nameof(PlaceChecking.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(PlaceChecking PlaceChecking)
        {
            return PlaceChecking.IsValidated;
        }

        public async Task<bool> Update(PlaceChecking PlaceChecking)
        {
            if (await ValidateId(PlaceChecking))
            {
            }
            return PlaceChecking.IsValidated;
        }

        public async Task<bool> Delete(PlaceChecking PlaceChecking)
        {
            if (await ValidateId(PlaceChecking))
            {
            }
            return PlaceChecking.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PlaceChecking> PlaceCheckings)
        {
            foreach (PlaceChecking PlaceChecking in PlaceCheckings)
            {
                await Delete(PlaceChecking);
            }
            return PlaceCheckings.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<PlaceChecking> PlaceCheckings)
        {
            return true;
        }
    }
}
