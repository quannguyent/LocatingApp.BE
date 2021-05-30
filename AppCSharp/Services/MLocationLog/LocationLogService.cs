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
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Services.MLocationLog
{
    public interface ILocationLogService :  IServiceScoped
    {
        Task<int> Count(LocationLogFilter LocationLogFilter);
        Task<List<LocationLog>> List(LocationLogFilter LocationLogFilter);
        Task<LocationLog> Get(long Id);
        Task<LocationLog> Create(LocationLog LocationLog);
        Task<LocationLog> Update(LocationLog LocationLog);
        Task<LocationLog> Delete(LocationLog LocationLog);
        Task<List<LocationLog>> BulkDelete(List<LocationLog> LocationLogs);
        Task<List<LocationLog>> Import(List<LocationLog> LocationLogs);
    }

    public class LocationLogService : BaseService, ILocationLogService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ILocationLogValidator LocationLogValidator;
        private IAppUserService AppUserService;

        public LocationLogService(
            IUOW UOW,
            IAppUserService AppUserService,
            ICurrentContext CurrentContext,
            ILocationLogValidator LocationLogValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.AppUserService = AppUserService;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.LocationLogValidator = LocationLogValidator;
        }
        public async Task<int> Count(LocationLogFilter LocationLogFilter)
        {
            try
            {
                LocationLogFilter.AppUserId = FilterBuilder.Merge(
                    LocationLogFilter.AppUserId, 
                    new IdFilter
                    {
                        In = await FilterAppUser(AppUserService, CurrentContext)
                    }); 
                int result = await UOW.LocationLogRepository.Count(LocationLogFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return 0;
        }

        public async Task<List<LocationLog>> List(LocationLogFilter LocationLogFilter)
        {
            try
            {
                LocationLogFilter.AppUserId = FilterBuilder.Merge(
                    LocationLogFilter.AppUserId,
                    new IdFilter
                    {
                        In = await FilterAppUser(AppUserService, CurrentContext)
                    });
                List<LocationLog> LocationLogs = await UOW.LocationLogRepository.List(LocationLogFilter);
                return LocationLogs;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;
        }
        
        public async Task<LocationLog> Get(long Id)
        {
            LocationLog LocationLog = await UOW.LocationLogRepository.Get(Id);
            if (LocationLog == null)
                return null;
            return LocationLog;
        }
        public async Task<LocationLog> Create(LocationLog LocationLog)
        {
            if (!await LocationLogValidator.Create(LocationLog))
                return LocationLog;

            try
            {
                await UOW.LocationLogRepository.Create(LocationLog);
                LocationLog = await UOW.LocationLogRepository.Get(LocationLog.Id);
                await Logging.CreateAuditLog(LocationLog, new { }, nameof(LocationLogService));
                return LocationLog;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;
        }

        public async Task<LocationLog> Update(LocationLog LocationLog)
        {
            if (!await LocationLogValidator.Update(LocationLog))
                return LocationLog;
            try
            {
                var oldData = await UOW.LocationLogRepository.Get(LocationLog.Id);

                await UOW.LocationLogRepository.Update(LocationLog);

                LocationLog = await UOW.LocationLogRepository.Get(LocationLog.Id);
                await Logging.CreateAuditLog(LocationLog, oldData, nameof(LocationLogService));
                return LocationLog;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;
        }

        public async Task<LocationLog> Delete(LocationLog LocationLog)
        {
            if (!await LocationLogValidator.Delete(LocationLog))
                return LocationLog;

            try
            {
                await UOW.LocationLogRepository.Delete(LocationLog);
                await Logging.CreateAuditLog(new { }, LocationLog, nameof(LocationLogService));
                return LocationLog;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;
        }

        public async Task<List<LocationLog>> BulkDelete(List<LocationLog> LocationLogs)
        {
            if (!await LocationLogValidator.BulkDelete(LocationLogs))
                return LocationLogs;

            try
            {
                await UOW.LocationLogRepository.BulkDelete(LocationLogs);
                await Logging.CreateAuditLog(new { }, LocationLogs, nameof(LocationLogService));
                return LocationLogs;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;

        }
        
        public async Task<List<LocationLog>> Import(List<LocationLog> LocationLogs)
        {
            if (!await LocationLogValidator.Import(LocationLogs))
                return LocationLogs;
            try
            {
                await UOW.LocationLogRepository.BulkMerge(LocationLogs);

                await Logging.CreateAuditLog(LocationLogs, new { }, nameof(LocationLogService));
                return LocationLogs;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(LocationLogService));
            }
            return null;
        }

        private async Task<List<long>> FilterAppUser(IAppUserService AppUserService, ICurrentContext CurrentContext)
        {
            AppUser AppUser = await AppUserService.Get(CurrentContext.UserId);
            List<AppUser> AppUsers = await AppUserService.ListFriends(AppUser);
            List<long> In = AppUsers.Select(x => x.Id).ToList();
            In.Add(CurrentContext.UserId);
            return In;
        }
    }
}
