using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp.Enums;
//using LocatingApp.Handlers;
using LocatingApp.Repositories;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LocatingApp.Helpers
{
    public interface ILogging : IServiceScoped
    {
        Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "");
        Task CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "");
    }
    public class Logging : ILogging
    {
        private ICurrentContext CurrentContext;
        private IUOW UOW; public Logging( ICurrentContext CurrentContext)
        {
            this.CurrentContext = CurrentContext;
        }
        public async Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "")
        {
            AuditLog AuditLog = new AuditLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = CurrentContext.UserName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                OldData = JsonConvert.SerializeObject(oldData),
                NewData = JsonConvert.SerializeObject(newData),
                Time = StaticParams.DateTimeNow,
                RowId = Guid.NewGuid(),
            };
            //RabbitManager.PublishSingle(new EventMessage<AuditLog>(AuditLog, AuditLog.RowId), RoutingKeyEnum.AuditLogSend);
            return true;
        }
        public async Task CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            if (ex.InnerException != null)
                ex = ex.InnerException;
            SystemLog SystemLog = new SystemLog
            {
                AppUserId = CurrentContext.UserId,
                AppUser = AppUser.DisplayName,
                ClassName = className,
                MethodName = methodName,
                ModuleName = StaticParams.ModuleName,
                Exception = ex.ToString(),
                Time = StaticParams.DateTimeNow,
            };

            throw new MessageException(ex);
        }
    }
}
