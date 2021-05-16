using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MPlace
{
    public interface IPlaceValidator : IServiceScoped
    {
        Task<bool> Create(Place Place);
        Task<bool> Update(Place Place);
        Task<bool> Delete(Place Place);
        Task<bool> BulkDelete(List<Place> Places);
        Task<bool> Import(List<Place> Places);
    }

    public class PlaceValidator : IPlaceValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PlaceValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Place Place)
        {
            PlaceFilter PlaceFilter = new PlaceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Place.Id },
                Selects = PlaceSelect.Id
            };

            int count = await UOW.PlaceRepository.Count(PlaceFilter);
            if (count == 0)
                Place.AddError(nameof(PlaceValidator), nameof(Place.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Place Place)
        {
            return Place.IsValidated;
        }

        public async Task<bool> Update(Place Place)
        {
            if (await ValidateId(Place))
            {
            }
            return Place.IsValidated;
        }

        public async Task<bool> Delete(Place Place)
        {
            if (await ValidateId(Place))
            {
            }
            return Place.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Place> Places)
        {
            foreach (Place Place in Places)
            {
                await Delete(Place);
            }
            return Places.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Place> Places)
        {
            return true;
        }
    }
}
