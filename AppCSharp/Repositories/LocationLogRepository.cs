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
    public interface ILocationLogRepository
    {
        Task<int> Count(LocationLogFilter LocationLogFilter);
        Task<List<LocationLog>> List(LocationLogFilter LocationLogFilter);
        Task<List<LocationLog>> List(List<long> Ids);
        Task<LocationLog> Get(long Id);
        Task<bool> Create(LocationLog LocationLog);
        Task<bool> Update(LocationLog LocationLog);
        Task<bool> Delete(LocationLog LocationLog);
        Task<bool> BulkMerge(List<LocationLog> LocationLogs);
        Task<bool> BulkDelete(List<LocationLog> LocationLogs);
    }
    public class LocationLogRepository : ILocationLogRepository
    {
        private DataContext DataContext;
        public LocationLogRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<LocationLogDAO> DynamicFilter(IQueryable<LocationLogDAO> query, LocationLogFilter filter)
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
            if (filter.PreviousId != null && filter.PreviousId.HasValue)
                query = query.Where(q => q.PreviousId.HasValue).Where(q => q.PreviousId.Value, filter.PreviousId);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.Latitude != null && filter.Latitude.HasValue)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longtitude != null && filter.Longtitude.HasValue)
                query = query.Where(q => q.Longtitude, filter.Longtitude);
            if (filter.UpdateInterval != null && filter.UpdateInterval.HasValue)
                query = query.Where(q => q.UpdateInterval, filter.UpdateInterval);
            //query = OrFilter(query, filter);
            return query;
        }

        //private IQueryable<LocationLogDAO> OrFilter(IQueryable<LocationLogDAO> query, LocationLogFilter filter)
        //{
        //    if (filter.OrFilter == null || filter.OrFilter.Count == 0)
        //        return query;
        //    IQueryable<LocationLogDAO> initQuery = query.Where(q => false);
        //    foreach (LocationLogFilter LocationLogFilter in filter.OrFilter)
        //    {
        //        IQueryable<LocationLogDAO> queryable = query;
        //        if (LocationLogFilter.Id != null && LocationLogFilter.Id.HasValue)
        //            queryable = queryable.Where(q => q.Id, filter.Id);
        //        if (LocationLogFilter.PreviousId != null && LocationLogFilter.PreviousId.HasValue)
        //            queryable = queryable.Where(q => q.PreviousId.HasValue).Where(q => q.PreviousId.Value, filter.PreviousId);
        //        if (LocationLogFilter.AppUserId != null && LocationLogFilter.AppUserId.HasValue)
        //            queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
        //        if (LocationLogFilter.Latitude != null && LocationLogFilter.Latitude.HasValue)
        //            queryable = queryable.Where(q => q.Latitude, filter.Latitude);
        //        if (LocationLogFilter.Longtitude != null && LocationLogFilter.Longtitude.HasValue)
        //            queryable = queryable.Where(q => q.Longtitude, filter.Longtitude);
        //        if (LocationLogFilter.UpdateInterval != null && LocationLogFilter.UpdateInterval.HasValue)
        //            queryable = queryable.Where(q => q.UpdateInterval, filter.UpdateInterval);
        //        initQuery = initQuery.Union(queryable);
        //    }
        //    return initQuery;
        //}    

        private IQueryable<LocationLogDAO> DynamicOrder(IQueryable<LocationLogDAO> query, LocationLogFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case LocationLogOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case LocationLogOrder.Previous:
                            query = query.OrderBy(q => q.PreviousId);
                            break;
                        case LocationLogOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case LocationLogOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case LocationLogOrder.Longtitude:
                            query = query.OrderBy(q => q.Longtitude);
                            break;
                        case LocationLogOrder.UpdateInterval:
                            query = query.OrderBy(q => q.UpdateInterval);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case LocationLogOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case LocationLogOrder.Previous:
                            query = query.OrderByDescending(q => q.PreviousId);
                            break;
                        case LocationLogOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case LocationLogOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case LocationLogOrder.Longtitude:
                            query = query.OrderByDescending(q => q.Longtitude);
                            break;
                        case LocationLogOrder.UpdateInterval:
                            query = query.OrderByDescending(q => q.UpdateInterval);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<LocationLog>> DynamicSelect(IQueryable<LocationLogDAO> query, LocationLogFilter filter)
        {
            List<LocationLog> LocationLogs = await query.Select(q => new LocationLog()
            {
                Id = filter.Selects.Contains(LocationLogSelect.Id) ? q.Id : default(long),
                PreviousId = filter.Selects.Contains(LocationLogSelect.Previous) ? q.PreviousId : default(long?),
                AppUserId = filter.Selects.Contains(LocationLogSelect.AppUser) ? q.AppUserId : default(long),
                Latitude = filter.Selects.Contains(LocationLogSelect.Latitude) ? q.Latitude : default(decimal),
                Longtitude = filter.Selects.Contains(LocationLogSelect.Longtitude) ? q.Longtitude : default(decimal),
                UpdateInterval = filter.Selects.Contains(LocationLogSelect.UpdateInterval) ? q.UpdateInterval : default(long),
                AppUser = filter.Selects.Contains(LocationLogSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    Password = q.AppUser.Password,
                    DisplayName = q.AppUser.DisplayName,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                } : null,
                Previous = filter.Selects.Contains(LocationLogSelect.Previous) && q.Previous != null ? new LocationLog
                {
                    Id = q.Previous.Id,
                    PreviousId = q.Previous.PreviousId,
                    AppUserId = q.Previous.AppUserId,
                    Latitude = q.Previous.Latitude,
                    Longtitude = q.Previous.Longtitude,
                    UpdateInterval = q.Previous.UpdateInterval,
                } : null,
            }).ToListAsync();
            return LocationLogs;
        }

        public async Task<int> Count(LocationLogFilter filter)
        {
            IQueryable<LocationLogDAO> LocationLogs = DataContext.LocationLog.AsNoTracking();
            LocationLogs = DynamicFilter(LocationLogs, filter);
            return await LocationLogs.CountAsync();
        }

        public async Task<List<LocationLog>> List(LocationLogFilter filter)
        {
            if (filter == null) return new List<LocationLog>();
            IQueryable<LocationLogDAO> LocationLogDAOs = DataContext.LocationLog.AsNoTracking();
            LocationLogDAOs = DynamicFilter(LocationLogDAOs, filter);
            LocationLogDAOs = DynamicOrder(LocationLogDAOs, filter);
            List<LocationLog> LocationLogs = await DynamicSelect(LocationLogDAOs, filter);
            return LocationLogs;
        }

        public async Task<List<LocationLog>> List(List<long> Ids)
        {
            List<LocationLog> LocationLogs = await DataContext.LocationLog.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new LocationLog()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                PreviousId = x.PreviousId,
                AppUserId = x.AppUserId,
                Latitude = x.Latitude,
                Longtitude = x.Longtitude,
                UpdateInterval = x.UpdateInterval,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    Password = x.AppUser.Password,
                    DisplayName = x.AppUser.DisplayName,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                },
                Previous = x.Previous == null ? null : new LocationLog
                {
                    Id = x.Previous.Id,
                    PreviousId = x.Previous.PreviousId,
                    AppUserId = x.Previous.AppUserId,
                    Latitude = x.Previous.Latitude,
                    Longtitude = x.Previous.Longtitude,
                    UpdateInterval = x.Previous.UpdateInterval,
                },
            }).ToListAsync();
            

            return LocationLogs;
        }

        public async Task<LocationLog> Get(long Id)
        {
            LocationLog LocationLog = await DataContext.LocationLog.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new LocationLog()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                PreviousId = x.PreviousId,
                AppUserId = x.AppUserId,
                Latitude = x.Latitude,
                Longtitude = x.Longtitude,
                UpdateInterval = x.UpdateInterval,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    Password = x.AppUser.Password,
                    DisplayName = x.AppUser.DisplayName,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                },
                Previous = x.Previous == null ? null : new LocationLog
                {
                    Id = x.Previous.Id,
                    PreviousId = x.Previous.PreviousId,
                    AppUserId = x.Previous.AppUserId,
                    Latitude = x.Previous.Latitude,
                    Longtitude = x.Previous.Longtitude,
                    UpdateInterval = x.Previous.UpdateInterval,
                },
            }).FirstOrDefaultAsync();

            if (LocationLog == null)
                return null;

            return LocationLog;
        }
        public async Task<bool> Create(LocationLog LocationLog)
        {
            LocationLogDAO Previous = await DataContext.LocationLog
                .Where(x => x.AppUserId == LocationLog.AppUserId)
                .FirstOrDefaultAsync();

            LocationLogDAO LocationLogDAO = new LocationLogDAO();
            LocationLogDAO.PreviousId = Previous?.Id;
            LocationLogDAO.AppUserId = LocationLog.AppUserId;
            LocationLogDAO.Latitude = LocationLog.Latitude;
            LocationLogDAO.Longtitude = LocationLog.Longtitude;
            LocationLogDAO.UpdateInterval = LocationLog.UpdateInterval;
            LocationLogDAO.CreatedAt = StaticParams.DateTimeNow;
            LocationLogDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.LocationLog.Add(LocationLogDAO);
            await DataContext.SaveChangesAsync();
            LocationLog.Id = LocationLogDAO.Id;
            await SaveReference(LocationLog);
            return true;
        }

        public async Task<bool> Update(LocationLog LocationLog)
        {
            LocationLogDAO LocationLogDAO = DataContext.LocationLog.Where(x => x.Id == LocationLog.Id).FirstOrDefault();
            if (LocationLogDAO == null)
                return false;
            LocationLogDAO.Id = LocationLog.Id;
            LocationLogDAO.PreviousId = LocationLog.PreviousId;
            LocationLogDAO.AppUserId = LocationLog.AppUserId;
            LocationLogDAO.Latitude = LocationLog.Latitude;
            LocationLogDAO.Longtitude = LocationLog.Longtitude;
            LocationLogDAO.UpdateInterval = LocationLog.UpdateInterval;
            LocationLogDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(LocationLog);
            return true;
        }

        public async Task<bool> Delete(LocationLog LocationLog)
        {
            await DataContext.LocationLog.Where(x => x.Id == LocationLog.Id).UpdateFromQueryAsync(x => new LocationLogDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<LocationLog> LocationLogs)
        {
            List<LocationLogDAO> LocationLogDAOs = new List<LocationLogDAO>();
            foreach (LocationLog LocationLog in LocationLogs)
            {
                LocationLogDAO LocationLogDAO = new LocationLogDAO();
                LocationLogDAO.Id = LocationLog.Id;
                LocationLogDAO.PreviousId = LocationLog.PreviousId;
                LocationLogDAO.AppUserId = LocationLog.AppUserId;
                LocationLogDAO.Latitude = LocationLog.Latitude;
                LocationLogDAO.Longtitude = LocationLog.Longtitude;
                LocationLogDAO.UpdateInterval = LocationLog.UpdateInterval;
                LocationLogDAO.CreatedAt = StaticParams.DateTimeNow;
                LocationLogDAO.UpdatedAt = StaticParams.DateTimeNow;
                LocationLogDAOs.Add(LocationLogDAO);
            }
            await DataContext.BulkMergeAsync(LocationLogDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<LocationLog> LocationLogs)
        {
            List<long> Ids = LocationLogs.Select(x => x.Id).ToList();
            await DataContext.LocationLog
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new LocationLogDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(LocationLog LocationLog)
        {
        }
        
    }
}
