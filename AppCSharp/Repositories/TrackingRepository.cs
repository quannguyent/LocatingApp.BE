using LocatingApp.Common;
using LocatingApp.Helpers;
using LocatingApp.Entities;
using LocatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Repositories
{
    public interface ITrackingRepository
    {
        Task<int> Count(TrackingFilter TrackingFilter);
        Task<List<Tracking>> List(TrackingFilter TrackingFilter);
        Task<List<Tracking>> List(List<long> Ids);
        Task<Tracking> Get(long Id);
        Task<bool> Create(Tracking Tracking);
        Task<bool> Update(Tracking Tracking);
        Task<bool> Delete(Tracking Tracking);
        Task<bool> BulkMerge(List<Tracking> Trackings);
        Task<bool> BulkDelete(List<Tracking> Trackings);
    }
    public class TrackingRepository : ITrackingRepository
    {
        private DataContext DataContext;
        public TrackingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<TrackingDAO> DynamicFilter(IQueryable<TrackingDAO> query, TrackingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.TrackerId != null && filter.TrackerId.HasValue)
                query = query.Where(q => q.TrackerId, filter.TrackerId);
            if (filter.TargetId != null && filter.TargetId.HasValue)
                query = query.Where(q => q.TargetId, filter.TargetId);
            if (filter.PlaceId != null && filter.PlaceId.HasValue)
                query = query.Where(q => q.PlaceId, filter.PlaceId);
            if (filter.PlaceCheckingId != null && filter.PlaceCheckingId.HasValue)
                query = query.Where(q => q.PlaceCheckingId, filter.PlaceCheckingId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<TrackingDAO> OrFilter(IQueryable<TrackingDAO> query, TrackingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<TrackingDAO> initQuery = query.Where(q => false);
            foreach (TrackingFilter TrackingFilter in filter.OrFilter)
            {
                IQueryable<TrackingDAO> queryable = query;
                if (TrackingFilter.Id != null && TrackingFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (TrackingFilter.TrackerId != null && TrackingFilter.TrackerId.HasValue)
                    queryable = queryable.Where(q => q.TrackerId, filter.TrackerId);
                if (TrackingFilter.TargetId != null && TrackingFilter.TargetId.HasValue)
                    queryable = queryable.Where(q => q.TargetId, filter.TargetId);
                if (TrackingFilter.PlaceId != null && TrackingFilter.PlaceId.HasValue)
                    queryable = queryable.Where(q => q.PlaceId, filter.PlaceId);
                if (TrackingFilter.PlaceCheckingId != null && TrackingFilter.PlaceCheckingId.HasValue)
                    queryable = queryable.Where(q => q.PlaceCheckingId, filter.PlaceCheckingId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<TrackingDAO> DynamicOrder(IQueryable<TrackingDAO> query, TrackingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case TrackingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case TrackingOrder.Tracker:
                            query = query.OrderBy(q => q.TrackerId);
                            break;
                        case TrackingOrder.Target:
                            query = query.OrderBy(q => q.TargetId);
                            break;
                        case TrackingOrder.Place:
                            query = query.OrderBy(q => q.PlaceId);
                            break;
                        case TrackingOrder.PlaceChecking:
                            query = query.OrderBy(q => q.PlaceCheckingId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case TrackingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case TrackingOrder.Tracker:
                            query = query.OrderByDescending(q => q.TrackerId);
                            break;
                        case TrackingOrder.Target:
                            query = query.OrderByDescending(q => q.TargetId);
                            break;
                        case TrackingOrder.Place:
                            query = query.OrderByDescending(q => q.PlaceId);
                            break;
                        case TrackingOrder.PlaceChecking:
                            query = query.OrderByDescending(q => q.PlaceCheckingId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Tracking>> DynamicSelect(IQueryable<TrackingDAO> query, TrackingFilter filter)
        {
            List<Tracking> Trackings = await query.Select(q => new Tracking()
            {
                Id = filter.Selects.Contains(TrackingSelect.Id) ? q.Id : default(long),
                TrackerId = filter.Selects.Contains(TrackingSelect.Tracker) ? q.TrackerId : default(long),
                TargetId = filter.Selects.Contains(TrackingSelect.Target) ? q.TargetId : default(long),
                PlaceId = filter.Selects.Contains(TrackingSelect.Place) ? q.PlaceId : default(long),
                PlaceCheckingId = filter.Selects.Contains(TrackingSelect.PlaceChecking) ? q.PlaceCheckingId : default(long),
                Place = filter.Selects.Contains(TrackingSelect.Place) && q.Place != null ? new Place
                {
                    Id = q.Place.Id,
                    Name = q.Place.Name,
                    PlaceGroupId = q.Place.PlaceGroupId,
                    Radius = q.Place.Radius,
                    Latitude = q.Place.Latitude,
                    Longtitude = q.Place.Longtitude,
                } : null,
                PlaceChecking = filter.Selects.Contains(TrackingSelect.PlaceChecking) && q.PlaceChecking != null ? new PlaceChecking
                {
                    Id = q.PlaceChecking.Id,
                    AppUserId = q.PlaceChecking.AppUserId,
                    PlaceId = q.PlaceChecking.PlaceId,
                    PlaceCheckingStatusId = q.PlaceChecking.PlaceCheckingStatusId,
                    CheckInAt = q.PlaceChecking.CheckInAt,
                    CheckOutAt = q.PlaceChecking.CheckOutAt,
                } : null,
                Target = filter.Selects.Contains(TrackingSelect.Target) && q.Target != null ? new AppUser
                {
                    Id = q.Target.Id,
                    Username = q.Target.Username,
                    Password = q.Target.Password,
                    DisplayName = q.Target.DisplayName,
                    Email = q.Target.Email,
                    Phone = q.Target.Phone,
                } : null,
                Tracker = filter.Selects.Contains(TrackingSelect.Tracker) && q.Tracker != null ? new AppUser
                {
                    Id = q.Tracker.Id,
                    Username = q.Tracker.Username,
                    Password = q.Tracker.Password,
                    DisplayName = q.Tracker.DisplayName,
                    Email = q.Tracker.Email,
                    Phone = q.Tracker.Phone,
                } : null,
            }).ToListAsync();
            return Trackings;
        }

        public async Task<int> Count(TrackingFilter filter)
        {
            IQueryable<TrackingDAO> Trackings = DataContext.Tracking.AsNoTracking();
            Trackings = DynamicFilter(Trackings, filter);
            return await Trackings.CountAsync();
        }

        public async Task<List<Tracking>> List(TrackingFilter filter)
        {
            if (filter == null) return new List<Tracking>();
            IQueryable<TrackingDAO> TrackingDAOs = DataContext.Tracking.AsNoTracking();
            TrackingDAOs = DynamicFilter(TrackingDAOs, filter);
            TrackingDAOs = DynamicOrder(TrackingDAOs, filter);
            List<Tracking> Trackings = await DynamicSelect(TrackingDAOs, filter);
            return Trackings;
        }

        public async Task<List<Tracking>> List(List<long> Ids)
        {
            List<Tracking> Trackings = await DataContext.Tracking.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Tracking()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                TrackerId = x.TrackerId,
                TargetId = x.TargetId,
                PlaceId = x.PlaceId,
                PlaceCheckingId = x.PlaceCheckingId,
                Place = x.Place == null ? null : new Place
                {
                    Id = x.Place.Id,
                    Name = x.Place.Name,
                    PlaceGroupId = x.Place.PlaceGroupId,
                    Radius = x.Place.Radius,
                    Latitude = x.Place.Latitude,
                    Longtitude = x.Place.Longtitude,
                },
                PlaceChecking = x.PlaceChecking == null ? null : new PlaceChecking
                {
                    Id = x.PlaceChecking.Id,
                    AppUserId = x.PlaceChecking.AppUserId,
                    PlaceId = x.PlaceChecking.PlaceId,
                    PlaceCheckingStatusId = x.PlaceChecking.PlaceCheckingStatusId,
                    CheckInAt = x.PlaceChecking.CheckInAt,
                    CheckOutAt = x.PlaceChecking.CheckOutAt,
                },
                Target = x.Target == null ? null : new AppUser
                {
                    Id = x.Target.Id,
                    Username = x.Target.Username,
                    Password = x.Target.Password,
                    DisplayName = x.Target.DisplayName,
                    Email = x.Target.Email,
                    Phone = x.Target.Phone,
                },
                Tracker = x.Tracker == null ? null : new AppUser
                {
                    Id = x.Tracker.Id,
                    Username = x.Tracker.Username,
                    Password = x.Tracker.Password,
                    DisplayName = x.Tracker.DisplayName,
                    Email = x.Tracker.Email,
                    Phone = x.Tracker.Phone,
                },
            }).ToListAsync();
            

            return Trackings;
        }

        public async Task<Tracking> Get(long Id)
        {
            Tracking Tracking = await DataContext.Tracking.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Tracking()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                TrackerId = x.TrackerId,
                TargetId = x.TargetId,
                PlaceId = x.PlaceId,
                PlaceCheckingId = x.PlaceCheckingId,
                Place = x.Place == null ? null : new Place
                {
                    Id = x.Place.Id,
                    Name = x.Place.Name,
                    PlaceGroupId = x.Place.PlaceGroupId,
                    Radius = x.Place.Radius,
                    Latitude = x.Place.Latitude,
                    Longtitude = x.Place.Longtitude,
                },
                PlaceChecking = x.PlaceChecking == null ? null : new PlaceChecking
                {
                    Id = x.PlaceChecking.Id,
                    AppUserId = x.PlaceChecking.AppUserId,
                    PlaceId = x.PlaceChecking.PlaceId,
                    PlaceCheckingStatusId = x.PlaceChecking.PlaceCheckingStatusId,
                    CheckInAt = x.PlaceChecking.CheckInAt,
                    CheckOutAt = x.PlaceChecking.CheckOutAt,
                },
                Target = x.Target == null ? null : new AppUser
                {
                    Id = x.Target.Id,
                    Username = x.Target.Username,
                    Password = x.Target.Password,
                    DisplayName = x.Target.DisplayName,
                    Email = x.Target.Email,
                    Phone = x.Target.Phone,
                },
                Tracker = x.Tracker == null ? null : new AppUser
                {
                    Id = x.Tracker.Id,
                    Username = x.Tracker.Username,
                    Password = x.Tracker.Password,
                    DisplayName = x.Tracker.DisplayName,
                    Email = x.Tracker.Email,
                    Phone = x.Tracker.Phone,
                },
            }).FirstOrDefaultAsync();

            if (Tracking == null)
                return null;

            return Tracking;
        }
        public async Task<bool> Create(Tracking Tracking)
        {
            TrackingDAO TrackingDAO = new TrackingDAO();
            TrackingDAO.Id = Tracking.Id;
            TrackingDAO.TrackerId = Tracking.TrackerId;
            TrackingDAO.TargetId = Tracking.TargetId;
            TrackingDAO.PlaceId = Tracking.PlaceId;
            TrackingDAO.PlaceCheckingId = Tracking.PlaceCheckingId;
            TrackingDAO.CreatedAt = StaticParams.DateTimeNow;
            TrackingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Tracking.Add(TrackingDAO);
            await DataContext.SaveChangesAsync();
            Tracking.Id = TrackingDAO.Id;
            await SaveReference(Tracking);
            return true;
        }

        public async Task<bool> Update(Tracking Tracking)
        {
            TrackingDAO TrackingDAO = DataContext.Tracking.Where(x => x.Id == Tracking.Id).FirstOrDefault();
            if (TrackingDAO == null)
                return false;
            TrackingDAO.Id = Tracking.Id;
            TrackingDAO.TrackerId = Tracking.TrackerId;
            TrackingDAO.TargetId = Tracking.TargetId;
            TrackingDAO.PlaceId = Tracking.PlaceId;
            TrackingDAO.PlaceCheckingId = Tracking.PlaceCheckingId;
            TrackingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Tracking);
            return true;
        }

        public async Task<bool> Delete(Tracking Tracking)
        {
            await DataContext.Tracking.Where(x => x.Id == Tracking.Id).UpdateFromQueryAsync(x => new TrackingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Tracking> Trackings)
        {
            List<TrackingDAO> TrackingDAOs = new List<TrackingDAO>();
            foreach (Tracking Tracking in Trackings)
            {
                TrackingDAO TrackingDAO = new TrackingDAO();
                TrackingDAO.Id = Tracking.Id;
                TrackingDAO.TrackerId = Tracking.TrackerId;
                TrackingDAO.TargetId = Tracking.TargetId;
                TrackingDAO.PlaceId = Tracking.PlaceId;
                TrackingDAO.PlaceCheckingId = Tracking.PlaceCheckingId;
                TrackingDAO.CreatedAt = StaticParams.DateTimeNow;
                TrackingDAO.UpdatedAt = StaticParams.DateTimeNow;
                TrackingDAOs.Add(TrackingDAO);
            }
            await DataContext.BulkMergeAsync(TrackingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Tracking> Trackings)
        {
            List<long> Ids = Trackings.Select(x => x.Id).ToList();
            await DataContext.Tracking
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new TrackingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Tracking Tracking)
        {
        }
        
    }
}
