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
    public interface IPlaceCheckingRepository
    {
        Task<int> Count(PlaceCheckingFilter PlaceCheckingFilter);
        Task<List<PlaceChecking>> List(PlaceCheckingFilter PlaceCheckingFilter);
        Task<List<PlaceChecking>> List(List<long> Ids);
        Task<PlaceChecking> Get(long Id);
        Task<bool> Create(PlaceChecking PlaceChecking);
        Task<bool> Update(PlaceChecking PlaceChecking);
        Task<bool> Delete(PlaceChecking PlaceChecking);
        Task<bool> BulkMerge(List<PlaceChecking> PlaceCheckings);
        Task<bool> BulkDelete(List<PlaceChecking> PlaceCheckings);
    }
    public class PlaceCheckingRepository : IPlaceCheckingRepository
    {
        private DataContext DataContext;
        public PlaceCheckingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PlaceCheckingDAO> DynamicFilter(IQueryable<PlaceCheckingDAO> query, PlaceCheckingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.AppUserId != null && filter.AppUserId.HasValue)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.PlaceId != null && filter.PlaceId.HasValue)
                query = query.Where(q => q.PlaceId, filter.PlaceId);
            if (filter.PlaceCheckingStatusId != null && filter.PlaceCheckingStatusId.HasValue)
                query = query.Where(q => q.PlaceCheckingStatusId, filter.PlaceCheckingStatusId);
            if (filter.CheckInAt != null && filter.CheckInAt.HasValue)
                query = query.Where(q => q.CheckInAt == null).Union(query.Where(q => q.CheckInAt.HasValue).Where(q => q.CheckInAt.Value, filter.CheckInAt));
            if (filter.CheckOutAt != null && filter.CheckOutAt.HasValue)
                query = query.Where(q => q.CheckOutAt == null).Union(query.Where(q => q.CheckOutAt.HasValue).Where(q => q.CheckOutAt.Value, filter.CheckOutAt));
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PlaceCheckingDAO> OrFilter(IQueryable<PlaceCheckingDAO> query, PlaceCheckingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PlaceCheckingDAO> initQuery = query.Where(q => false);
            foreach (PlaceCheckingFilter PlaceCheckingFilter in filter.OrFilter)
            {
                IQueryable<PlaceCheckingDAO> queryable = query;
                if (PlaceCheckingFilter.Id != null && PlaceCheckingFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (PlaceCheckingFilter.AppUserId != null && PlaceCheckingFilter.AppUserId.HasValue)
                    queryable = queryable.Where(q => q.AppUserId, filter.AppUserId);
                if (PlaceCheckingFilter.PlaceId != null && PlaceCheckingFilter.PlaceId.HasValue)
                    queryable = queryable.Where(q => q.PlaceId, filter.PlaceId);
                if (PlaceCheckingFilter.PlaceCheckingStatusId != null && PlaceCheckingFilter.PlaceCheckingStatusId.HasValue)
                    queryable = queryable.Where(q => q.PlaceCheckingStatusId, filter.PlaceCheckingStatusId);
                if (PlaceCheckingFilter.CheckInAt != null && PlaceCheckingFilter.CheckInAt.HasValue)
                    queryable = queryable.Where(q => q.CheckInAt == null).Union(queryable.Where(q => q.CheckInAt.HasValue).Where(q => q.CheckInAt.Value, filter.CheckInAt));
                if (PlaceCheckingFilter.CheckOutAt != null && PlaceCheckingFilter.CheckOutAt.HasValue)
                    queryable = queryable.Where(q => q.CheckOutAt == null).Union(queryable.Where(q => q.CheckOutAt.HasValue).Where(q => q.CheckOutAt.Value, filter.CheckOutAt));
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PlaceCheckingDAO> DynamicOrder(IQueryable<PlaceCheckingDAO> query, PlaceCheckingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PlaceCheckingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PlaceCheckingOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case PlaceCheckingOrder.Place:
                            query = query.OrderBy(q => q.PlaceId);
                            break;
                        case PlaceCheckingOrder.PlaceCheckingStatus:
                            query = query.OrderBy(q => q.PlaceCheckingStatusId);
                            break;
                        case PlaceCheckingOrder.CheckInAt:
                            query = query.OrderBy(q => q.CheckInAt);
                            break;
                        case PlaceCheckingOrder.CheckOutAt:
                            query = query.OrderBy(q => q.CheckOutAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PlaceCheckingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PlaceCheckingOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case PlaceCheckingOrder.Place:
                            query = query.OrderByDescending(q => q.PlaceId);
                            break;
                        case PlaceCheckingOrder.PlaceCheckingStatus:
                            query = query.OrderByDescending(q => q.PlaceCheckingStatusId);
                            break;
                        case PlaceCheckingOrder.CheckInAt:
                            query = query.OrderByDescending(q => q.CheckInAt);
                            break;
                        case PlaceCheckingOrder.CheckOutAt:
                            query = query.OrderByDescending(q => q.CheckOutAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PlaceChecking>> DynamicSelect(IQueryable<PlaceCheckingDAO> query, PlaceCheckingFilter filter)
        {
            List<PlaceChecking> PlaceCheckings = await query.Select(q => new PlaceChecking()
            {
                Id = filter.Selects.Contains(PlaceCheckingSelect.Id) ? q.Id : default(long),
                AppUserId = filter.Selects.Contains(PlaceCheckingSelect.AppUser) ? q.AppUserId : default(long),
                PlaceId = filter.Selects.Contains(PlaceCheckingSelect.Place) ? q.PlaceId : default(long),
                PlaceCheckingStatusId = filter.Selects.Contains(PlaceCheckingSelect.PlaceCheckingStatus) ? q.PlaceCheckingStatusId : default(long),
                CheckInAt = filter.Selects.Contains(PlaceCheckingSelect.CheckInAt) ? q.CheckInAt : default(DateTime?),
                CheckOutAt = filter.Selects.Contains(PlaceCheckingSelect.CheckOutAt) ? q.CheckOutAt : default(DateTime?),
                AppUser = filter.Selects.Contains(PlaceCheckingSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    Password = q.AppUser.Password,
                    DisplayName = q.AppUser.DisplayName,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                } : null,
                Place = filter.Selects.Contains(PlaceCheckingSelect.Place) && q.Place != null ? new Place
                {
                    Id = q.Place.Id,
                    Name = q.Place.Name,
                    PlaceGroupId = q.Place.PlaceGroupId,
                    Radius = q.Place.Radius,
                    Latitude = q.Place.Latitude,
                    Longtitude = q.Place.Longtitude,
                } : null,
                PlaceCheckingStatus = filter.Selects.Contains(PlaceCheckingSelect.PlaceCheckingStatus) && q.PlaceCheckingStatus != null ? new CheckingStatus
                {
                    Id = q.PlaceCheckingStatus.Id,
                    Code = q.PlaceCheckingStatus.Code,
                    Name = q.PlaceCheckingStatus.Name,
                } : null,
            }).ToListAsync();
            return PlaceCheckings;
        }

        public async Task<int> Count(PlaceCheckingFilter filter)
        {
            IQueryable<PlaceCheckingDAO> PlaceCheckings = DataContext.PlaceChecking.AsNoTracking();
            PlaceCheckings = DynamicFilter(PlaceCheckings, filter);
            return await PlaceCheckings.CountAsync();
        }

        public async Task<List<PlaceChecking>> List(PlaceCheckingFilter filter)
        {
            if (filter == null) return new List<PlaceChecking>();
            IQueryable<PlaceCheckingDAO> PlaceCheckingDAOs = DataContext.PlaceChecking.AsNoTracking();
            PlaceCheckingDAOs = DynamicFilter(PlaceCheckingDAOs, filter);
            PlaceCheckingDAOs = DynamicOrder(PlaceCheckingDAOs, filter);
            List<PlaceChecking> PlaceCheckings = await DynamicSelect(PlaceCheckingDAOs, filter);
            return PlaceCheckings;
        }

        public async Task<List<PlaceChecking>> List(List<long> Ids)
        {
            List<PlaceChecking> PlaceCheckings = await DataContext.PlaceChecking.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new PlaceChecking()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                PlaceId = x.PlaceId,
                PlaceCheckingStatusId = x.PlaceCheckingStatusId,
                CheckInAt = x.CheckInAt,
                CheckOutAt = x.CheckOutAt,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    Password = x.AppUser.Password,
                    DisplayName = x.AppUser.DisplayName,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                },
                Place = x.Place == null ? null : new Place
                {
                    Id = x.Place.Id,
                    Name = x.Place.Name,
                    PlaceGroupId = x.Place.PlaceGroupId,
                    Radius = x.Place.Radius,
                    Latitude = x.Place.Latitude,
                    Longtitude = x.Place.Longtitude,
                },
                PlaceCheckingStatus = x.PlaceCheckingStatus == null ? null : new CheckingStatus
                {
                    Id = x.PlaceCheckingStatus.Id,
                    Code = x.PlaceCheckingStatus.Code,
                    Name = x.PlaceCheckingStatus.Name,
                },
            }).ToListAsync();
            

            return PlaceCheckings;
        }

        public async Task<PlaceChecking> Get(long Id)
        {
            PlaceChecking PlaceChecking = await DataContext.PlaceChecking.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new PlaceChecking()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                PlaceId = x.PlaceId,
                PlaceCheckingStatusId = x.PlaceCheckingStatusId,
                CheckInAt = x.CheckInAt,
                CheckOutAt = x.CheckOutAt,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    Password = x.AppUser.Password,
                    DisplayName = x.AppUser.DisplayName,
                    Email = x.AppUser.Email,
                    Phone = x.AppUser.Phone,
                },
                Place = x.Place == null ? null : new Place
                {
                    Id = x.Place.Id,
                    Name = x.Place.Name,
                    PlaceGroupId = x.Place.PlaceGroupId,
                    Radius = x.Place.Radius,
                    Latitude = x.Place.Latitude,
                    Longtitude = x.Place.Longtitude,
                },
                PlaceCheckingStatus = x.PlaceCheckingStatus == null ? null : new CheckingStatus
                {
                    Id = x.PlaceCheckingStatus.Id,
                    Code = x.PlaceCheckingStatus.Code,
                    Name = x.PlaceCheckingStatus.Name,
                },
            }).FirstOrDefaultAsync();

            if (PlaceChecking == null)
                return null;

            return PlaceChecking;
        }
        public async Task<bool> Create(PlaceChecking PlaceChecking)
        {
            PlaceCheckingDAO PlaceCheckingDAO = new PlaceCheckingDAO();
            PlaceCheckingDAO.Id = PlaceChecking.Id;
            PlaceCheckingDAO.AppUserId = PlaceChecking.AppUserId;
            PlaceCheckingDAO.PlaceId = PlaceChecking.PlaceId;
            PlaceCheckingDAO.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
            PlaceCheckingDAO.CheckInAt = PlaceChecking.CheckInAt;
            PlaceCheckingDAO.CheckOutAt = PlaceChecking.CheckOutAt;
            DataContext.PlaceChecking.Add(PlaceCheckingDAO);
            await DataContext.SaveChangesAsync();
            PlaceChecking.Id = PlaceCheckingDAO.Id;
            await SaveReference(PlaceChecking);
            return true;
        }

        public async Task<bool> Update(PlaceChecking PlaceChecking)
        {
            PlaceCheckingDAO PlaceCheckingDAO = DataContext.PlaceChecking.Where(x => x.Id == PlaceChecking.Id).FirstOrDefault();
            if (PlaceCheckingDAO == null)
                return false;
            PlaceCheckingDAO.Id = PlaceChecking.Id;
            PlaceCheckingDAO.AppUserId = PlaceChecking.AppUserId;
            PlaceCheckingDAO.PlaceId = PlaceChecking.PlaceId;
            PlaceCheckingDAO.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
            PlaceCheckingDAO.CheckInAt = PlaceChecking.CheckInAt;
            PlaceCheckingDAO.CheckOutAt = PlaceChecking.CheckOutAt;
            await DataContext.SaveChangesAsync();
            await SaveReference(PlaceChecking);
            return true;
        }

        public async Task<bool> Delete(PlaceChecking PlaceChecking)
        {
            await DataContext.PlaceChecking.Where(x => x.Id == PlaceChecking.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PlaceChecking> PlaceCheckings)
        {
            List<PlaceCheckingDAO> PlaceCheckingDAOs = new List<PlaceCheckingDAO>();
            foreach (PlaceChecking PlaceChecking in PlaceCheckings)
            {
                PlaceCheckingDAO PlaceCheckingDAO = new PlaceCheckingDAO();
                PlaceCheckingDAO.Id = PlaceChecking.Id;
                PlaceCheckingDAO.AppUserId = PlaceChecking.AppUserId;
                PlaceCheckingDAO.PlaceId = PlaceChecking.PlaceId;
                PlaceCheckingDAO.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
                PlaceCheckingDAO.CheckInAt = PlaceChecking.CheckInAt;
                PlaceCheckingDAO.CheckOutAt = PlaceChecking.CheckOutAt;
                PlaceCheckingDAOs.Add(PlaceCheckingDAO);
            }
            await DataContext.BulkMergeAsync(PlaceCheckingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PlaceChecking> PlaceCheckings)
        {
            List<long> Ids = PlaceCheckings.Select(x => x.Id).ToList();
            await DataContext.PlaceChecking
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PlaceChecking PlaceChecking)
        {
        }
        
    }
}
