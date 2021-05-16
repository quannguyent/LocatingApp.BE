using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MTracking
{
    public interface ITrackingValidator : IServiceScoped
    {
        Task<bool> Create(Tracking Tracking);
        Task<bool> Update(Tracking Tracking);
        Task<bool> Delete(Tracking Tracking);
        Task<bool> BulkDelete(List<Tracking> Trackings);
        Task<bool> Import(List<Tracking> Trackings);
    }

    public class TrackingValidator : ITrackingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public TrackingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Tracking Tracking)
        {
            TrackingFilter TrackingFilter = new TrackingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Tracking.Id },
                Selects = TrackingSelect.Id
            };

            int count = await UOW.TrackingRepository.Count(TrackingFilter);
            if (count == 0)
                Tracking.AddError(nameof(TrackingValidator), nameof(Tracking.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Tracking Tracking)
        {
            return Tracking.IsValidated;
        }

        public async Task<bool> Update(Tracking Tracking)
        {
            if (await ValidateId(Tracking))
            {
            }
            return Tracking.IsValidated;
        }

        public async Task<bool> Delete(Tracking Tracking)
        {
            if (await ValidateId(Tracking))
            {
            }
            return Tracking.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Tracking> Trackings)
        {
            foreach (Tracking Tracking in Trackings)
            {
                await Delete(Tracking);
            }
            return Trackings.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<Tracking> Trackings)
        {
            return true;
        }
    }
}
