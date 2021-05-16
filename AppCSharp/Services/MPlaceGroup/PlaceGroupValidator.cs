using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MPlaceGroup
{
    public interface IPlaceGroupValidator : IServiceScoped
    {
        Task<bool> Create(PlaceGroup PlaceGroup);
        Task<bool> Update(PlaceGroup PlaceGroup);
        Task<bool> Delete(PlaceGroup PlaceGroup);
        Task<bool> BulkDelete(List<PlaceGroup> PlaceGroups);
        Task<bool> Import(List<PlaceGroup> PlaceGroups);
    }

    public class PlaceGroupValidator : IPlaceGroupValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PlaceGroupValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(PlaceGroup PlaceGroup)
        {
            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = PlaceGroup.Id },
                Selects = PlaceGroupSelect.Id
            };

            int count = await UOW.PlaceGroupRepository.Count(PlaceGroupFilter);
            if (count == 0)
                PlaceGroup.AddError(nameof(PlaceGroupValidator), nameof(PlaceGroup.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(PlaceGroup PlaceGroup)
        {
            return PlaceGroup.IsValidated;
        }

        public async Task<bool> Update(PlaceGroup PlaceGroup)
        {
            if (await ValidateId(PlaceGroup))
            {
            }
            return PlaceGroup.IsValidated;
        }

        public async Task<bool> Delete(PlaceGroup PlaceGroup)
        {
            if (await ValidateId(PlaceGroup))
            {
            }
            return PlaceGroup.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<PlaceGroup> PlaceGroups)
        {
            foreach (PlaceGroup PlaceGroup in PlaceGroups)
            {
                await Delete(PlaceGroup);
            }
            return PlaceGroups.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<PlaceGroup> PlaceGroups)
        {
            return true;
        }
    }
}
