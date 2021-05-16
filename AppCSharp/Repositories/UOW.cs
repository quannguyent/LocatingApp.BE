using LocatingApp.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using LocatingApp.Models;
using LocatingApp.Repositories;
using System;

namespace LocatingApp.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAppUserRepository AppUserRepository { get; }
        ICheckingStatusRepository CheckingStatusRepository { get; }
        ILocationLogRepository LocationLogRepository { get; }
        IPlaceCheckingRepository PlaceCheckingRepository { get; }
        IPlaceRepository PlaceRepository { get; }
        IPlaceGroupRepository PlaceGroupRepository { get; }
        ISexRepository SexRepository { get; }
        ITrackingRepository TrackingRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAppUserRepository AppUserRepository { get; private set; }
        public ICheckingStatusRepository CheckingStatusRepository { get; private set; }
        public ILocationLogRepository LocationLogRepository { get; private set; }
        public IPlaceCheckingRepository PlaceCheckingRepository { get; private set; }
        public IPlaceRepository PlaceRepository { get; private set; }
        public IPlaceGroupRepository PlaceGroupRepository { get; private set; }
        public ISexRepository SexRepository { get; private set; }
        public ITrackingRepository TrackingRepository { get; private set; }

        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            CheckingStatusRepository = new CheckingStatusRepository(DataContext);
            LocationLogRepository = new LocationLogRepository(DataContext);
            PlaceCheckingRepository = new PlaceCheckingRepository(DataContext);
            PlaceRepository = new PlaceRepository(DataContext);
            PlaceGroupRepository = new PlaceGroupRepository(DataContext);
            SexRepository = new SexRepository(DataContext);
            TrackingRepository = new TrackingRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            //DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            //DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }

            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}