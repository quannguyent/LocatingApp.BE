using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;

namespace LocatingApp.Services.MLocationLog
{
    public interface ILocationLogValidator : IServiceScoped
    {
        Task<bool> Create(LocationLog LocationLog);
        Task<bool> Update(LocationLog LocationLog);
        Task<bool> Delete(LocationLog LocationLog);
        Task<bool> BulkDelete(List<LocationLog> LocationLogs);
        Task<bool> Import(List<LocationLog> LocationLogs);
    }

    public class LocationLogValidator : ILocationLogValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            NotCurrentUser,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public LocationLogValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(LocationLog LocationLog)
        {
            LocationLogFilter LocationLogFilter = new LocationLogFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = LocationLog.Id },
                Selects = LocationLogSelect.Id
            };

            int count = await UOW.LocationLogRepository.Count(LocationLogFilter);
            if (count == 0)
                LocationLog.AddError(nameof(LocationLogValidator), nameof(LocationLog.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateUser(LocationLog LocationLog)
        {
            if (LocationLog.AppUserId != CurrentContext.UserId)
                LocationLog.AddError(nameof(LocationLogValidator), nameof(LocationLog.Id), ErrorCode.NotCurrentUser);
            return LocationLog.AppUserId == CurrentContext.UserId;
        }

        public async Task<bool> Create(LocationLog LocationLog)
        {
            await ValidateUser(LocationLog);
            return LocationLog.IsValidated;
        }

        public async Task<bool> Update(LocationLog LocationLog)
        {
            if (await ValidateId(LocationLog))
            {
            }
            return LocationLog.IsValidated;
        }

        public async Task<bool> Delete(LocationLog LocationLog)
        {
            if (await ValidateId(LocationLog))
            {
            }
            return LocationLog.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<LocationLog> LocationLogs)
        {
            foreach (LocationLog LocationLog in LocationLogs)
            {
                await Delete(LocationLog);
            }
            return LocationLogs.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<LocationLog> LocationLogs)
        {
            return true;
        }
    }
}
