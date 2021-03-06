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
    public interface IPlaceRepository
    {
        Task<int> Count(PlaceFilter PlaceFilter);
        Task<List<Place>> List(PlaceFilter PlaceFilter);
        Task<List<Place>> List(List<long> Ids);
        Task<Place> Get(long Id);
        Task<bool> Create(Place Place);
        Task<bool> Update(Place Place);
        Task<bool> Delete(Place Place);
        Task<bool> BulkMerge(List<Place> Places);
        Task<bool> BulkDelete(List<Place> Places);
    }
    public class PlaceRepository : IPlaceRepository
    {
        private DataContext DataContext;
        public PlaceRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PlaceDAO> DynamicFilter(IQueryable<PlaceDAO> query, PlaceFilter filter)
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
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.PlaceGroupId != null && filter.PlaceGroupId.HasValue)
                query = query.Where(q => q.PlaceGroupId.HasValue).Where(q => q.PlaceGroupId.Value, filter.PlaceGroupId);
            if (filter.Radius != null && filter.Radius.HasValue)
                query = query.Where(q => q.Radius, filter.Radius);
            if (filter.Latitude != null && filter.Latitude.HasValue)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longtitude != null && filter.Longtitude.HasValue)
                query = query.Where(q => q.Longtitude, filter.Longtitude);
            return query;
        }
        private IQueryable<PlaceDAO> DynamicOrder(IQueryable<PlaceDAO> query, PlaceFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PlaceOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PlaceOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PlaceOrder.PlaceGroup:
                            query = query.OrderBy(q => q.PlaceGroupId);
                            break;
                        case PlaceOrder.Radius:
                            query = query.OrderBy(q => q.Radius);
                            break;
                        case PlaceOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case PlaceOrder.Longtitude:
                            query = query.OrderBy(q => q.Longtitude);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PlaceOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PlaceOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PlaceOrder.PlaceGroup:
                            query = query.OrderByDescending(q => q.PlaceGroupId);
                            break;
                        case PlaceOrder.Radius:
                            query = query.OrderByDescending(q => q.Radius);
                            break;
                        case PlaceOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case PlaceOrder.Longtitude:
                            query = query.OrderByDescending(q => q.Longtitude);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Place>> DynamicSelect(IQueryable<PlaceDAO> query, PlaceFilter filter)
        {
            List<Place> Places = await query.Select(q => new Place()
            {
                Id = filter.Selects.Contains(PlaceSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(PlaceSelect.Name) ? q.Name : default(string),
                Code = filter.Selects.Contains(PlaceSelect.Code) ? q.Code : default(string),
                PlaceGroupId = filter.Selects.Contains(PlaceSelect.PlaceGroup) ? q.PlaceGroupId : default(long?),
                Radius = filter.Selects.Contains(PlaceSelect.Radius) ? q.Radius : default(long),
                Latitude = filter.Selects.Contains(PlaceSelect.Latitude) ? q.Latitude : default(decimal),
                Longtitude = filter.Selects.Contains(PlaceSelect.Longtitude) ? q.Longtitude : default(decimal),
            }).ToListAsync();
            return Places;
        }

        public async Task<int> Count(PlaceFilter filter)
        {
            IQueryable<PlaceDAO> Places = DataContext.Place.AsNoTracking();
            Places = DynamicFilter(Places, filter);
            return await Places.CountAsync();
        }

        public async Task<List<Place>> List(PlaceFilter filter)
        {
            if (filter == null) return new List<Place>();
            IQueryable<PlaceDAO> PlaceDAOs = DataContext.Place.AsNoTracking();
            PlaceDAOs = DynamicFilter(PlaceDAOs, filter);
            PlaceDAOs = DynamicOrder(PlaceDAOs, filter);
            List<Place> Places = await DynamicSelect(PlaceDAOs, filter);
            return Places;
        }

        public async Task<List<Place>> List(List<long> Ids)
        {
            List<Place> Places = await DataContext.Place.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Place()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                PlaceGroupId = x.PlaceGroupId,
                Radius = x.Radius,
                Latitude = x.Latitude,
                Longtitude = x.Longtitude,
            }).ToListAsync();
            List<PlaceChecking> PlaceCheckings = await DataContext.PlaceChecking.AsNoTracking()
                .Where(x => Ids.Contains(x.PlaceId)).Select(x => new PlaceChecking
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    PlaceCheckingStatusId = x.PlaceCheckingStatusId,
                    PlaceId = x.PlaceId,
                    AppUser = new AppUser
                    {
                        Id = x.AppUser.Id,
                        DisplayName = x.AppUser.DisplayName,
                    },
                    CheckInAt = x.CheckInAt,
                    CheckOutAt = x.CheckOutAt,
                }).ToListAsync();
            foreach (Place Place in Places)
            {
                Place.PlaceCheckings = PlaceCheckings.Where(x => x.PlaceId == Place.Id).ToList();
            }
            return Places;
        }

        public async Task<Place> Get(long Id)
        {
            Place Place = await DataContext.Place.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Place()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                PlaceGroupId = x.PlaceGroupId,
                Radius = x.Radius,
                Latitude = x.Latitude,
                Longtitude = x.Longtitude,
            }).FirstOrDefaultAsync();

            if (Place == null)
                return null;

            return Place;
        }
        public async Task<bool> Create(Place Place)
        {
            PlaceDAO PlaceDAO = new PlaceDAO();
            PlaceDAO.Id = Place.Id;
            PlaceDAO.Name = Place.Name;
            PlaceDAO.Code = Place.Code;
            PlaceDAO.PlaceGroupId = Place.PlaceGroupId;
            PlaceDAO.Radius = Place.Radius;
            PlaceDAO.Latitude = Place.Latitude;
            PlaceDAO.Longtitude = Place.Longtitude;
            PlaceDAO.CreatedAt = StaticParams.DateTimeNow;
            PlaceDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Place.Add(PlaceDAO);
            await DataContext.SaveChangesAsync();
            Place.Id = PlaceDAO.Id;
            await SaveReference(Place);
            return true;
        }

        public async Task<bool> Update(Place Place)
        {
            PlaceDAO PlaceDAO = DataContext.Place.Where(x => x.Id == Place.Id).FirstOrDefault();
            if (PlaceDAO == null)
                return false;
            PlaceDAO.Id = Place.Id;
            PlaceDAO.Name = Place.Name;
            PlaceDAO.Code = Place.Code;
            PlaceDAO.PlaceGroupId = Place.PlaceGroupId;
            PlaceDAO.Radius = Place.Radius;
            PlaceDAO.Latitude = Place.Latitude;
            PlaceDAO.Longtitude = Place.Longtitude;
            PlaceDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Place);
            return true;
        }

        public async Task<bool> Delete(Place Place)
        {
            await DataContext.Place.Where(x => x.Id == Place.Id).UpdateFromQueryAsync(x => new PlaceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Place> Places)
        {
            List<PlaceDAO> PlaceDAOs = await DataContext.Place.ToListAsync();
            foreach (Place Place in Places)
            {
                PlaceDAO PlaceDAO = new PlaceDAO();
                PlaceDAO.Id = Place.Id;
                PlaceDAO.Code = Place.Code;
                PlaceDAO.Name = Place.Name;
                PlaceDAO.PlaceGroupId = Place.PlaceGroupId;
                PlaceDAO.Radius = Place.Radius;
                PlaceDAO.Latitude = Place.Latitude;
                PlaceDAO.Longtitude = Place.Longtitude;
                PlaceDAO.CreatedAt = StaticParams.DateTimeNow;
                PlaceDAO.UpdatedAt = StaticParams.DateTimeNow;

                if (!PlaceDAOs.Exists(x => x.Code == Place.Code))
                    PlaceDAOs.Add(PlaceDAO);
            }
            await DataContext.BulkMergeAsync(PlaceDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Place> Places)
        {
            List<long> Ids = Places.Select(x => x.Id).ToList();
            await DataContext.Place
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PlaceDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Place Place)
        {
            List<PlaceCheckingDAO> PlaceCheckingDAOs = await DataContext.PlaceChecking
                .Where(x => x.PlaceId == Place.Id).ToListAsync();
            if (Place.PlaceCheckings != null)
            {
                foreach (PlaceChecking PlaceChecking in Place.PlaceCheckings)
                {
                    PlaceCheckingDAO PlaceCheckingDAO = PlaceCheckingDAOs
                        .Where(x => x.PlaceId == PlaceChecking.PlaceId &&
                        x.AppUserId == PlaceChecking.AppUserId &&
                        x.PlaceId != 0 && x.AppUserId != 0)
                        .FirstOrDefault();
                    if (PlaceCheckingDAO == null)
                    {
                        PlaceCheckingDAO = new PlaceCheckingDAO();
                        PlaceCheckingDAO.AppUserId = PlaceChecking.AppUserId;
                        PlaceCheckingDAO.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
                        PlaceCheckingDAO.PlaceId = Place.Id;
                        PlaceCheckingDAO.CheckInAt = PlaceChecking.CheckInAt;
                        PlaceCheckingDAO.CheckOutAt = PlaceChecking.CheckOutAt;
                        PlaceCheckingDAOs.Add(PlaceCheckingDAO);
                    }
                    else
                    {
                        PlaceCheckingDAO.AppUserId = PlaceChecking.AppUserId;
                        PlaceCheckingDAO.PlaceCheckingStatusId = PlaceChecking.PlaceCheckingStatusId;
                        PlaceCheckingDAO.PlaceId = Place.Id;
                        PlaceCheckingDAO.CheckInAt = PlaceChecking.CheckInAt;
                        PlaceCheckingDAO.CheckOutAt = PlaceChecking.CheckOutAt;
                    }
                }
                await DataContext.PlaceChecking.BulkMergeAsync(PlaceCheckingDAOs);
            }
        }
    }
}
