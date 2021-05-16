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

namespace LocatingApp.Services.MSex
{
    public interface ISexService :  IServiceScoped
    {
        Task<int> Count(SexFilter SexFilter);
        Task<List<Sex>> List(SexFilter SexFilter);
    }

    public class SexService : BaseService, ISexService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISexValidator SexValidator;

        public SexService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISexValidator SexValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SexValidator = SexValidator;
        }
        public async Task<int> Count(SexFilter SexFilter)
        {
            try
            {
                int result = await UOW.SexRepository.Count(SexFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SexService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Sex>> List(SexFilter SexFilter)
        {
            try
            {
                List<Sex> Sexs = await UOW.SexRepository.List(SexFilter);
                return Sexs;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SexService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
