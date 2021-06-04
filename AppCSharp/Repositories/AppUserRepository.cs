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
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AppUserDAO> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
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
            if (filter.Username != null && filter.Username.HasValue)
                query = query.Where(q => q.Username, filter.Username);
            if (filter.Password != null && filter.Password.HasValue)
                query = query.Where(q => q.Password, filter.Password);
            if (filter.DisplayName != null && filter.DisplayName.HasValue)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.Email != null && filter.Email.HasValue)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Phone != null && filter.Phone.HasValue)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.SexId != null && filter.SexId.HasValue)
                query = query.Where(q => q.SexId, filter.SexId);
            if (filter.Birthday != null && filter.Birthday.HasValue)
                query = query.Where(q => q.Birthday, filter.Birthday);
            if (filter.RoleId != null && filter.RoleId.HasValue)
                query = query.Where(q => q.RoleId, filter.RoleId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<AppUserDAO> OrFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AppUserDAO> initQuery = query.Where(q => false);
            foreach (AppUserFilter AppUserFilter in filter.OrFilter)
            {
                IQueryable<AppUserDAO> queryable = query;
                if (AppUserFilter.Id != null && AppUserFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (AppUserFilter.Username != null && AppUserFilter.Username.HasValue)
                    queryable = queryable.Where(q => q.Username, filter.Username);
                if (AppUserFilter.Password != null && AppUserFilter.Password.HasValue)
                    queryable = queryable.Where(q => q.Password, filter.Password);
                if (AppUserFilter.DisplayName != null && AppUserFilter.DisplayName.HasValue)
                    queryable = queryable.Where(q => q.DisplayName, filter.DisplayName);
                if (AppUserFilter.Email != null && AppUserFilter.Email.HasValue)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (AppUserFilter.Phone != null && AppUserFilter.Phone.HasValue)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (AppUserFilter.SexId != null && AppUserFilter.SexId.HasValue)
                    queryable = queryable.Where(q => q.SexId, filter.SexId);
                if (AppUserFilter.Birthday != null && AppUserFilter.Birthday.HasValue)
                    queryable = queryable.Where(q => q.Birthday, filter.Birthday);
                if (AppUserFilter.RoleId != null && AppUserFilter.RoleId.HasValue)
                    queryable = queryable.Where(q => q.RoleId, filter.RoleId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AppUserOrder.Sex:
                                query = query.OrderBy(q => q.SexId);
                                break;
                        case AppUserOrder.Birthday:
                                query = query.OrderBy(q => q.Birthday);
                                break;
                        case AppUserOrder.Role:
                                query = query.OrderBy(q => q.RoleId);
                                break;

                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderBy(q => q.SexId);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                        case AppUserOrder.Role:
                            query = query.OrderBy(q => q.RoleId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AppUserSelect.Birthday) ? q.Birthday : default(DateTime),
                RoleId = filter.Selects.Contains(AppUserSelect.Role) ? q.RoleId : default(long),
            }).ToListAsync();
            return AppUsers;
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUser.AsNoTracking();
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            List<AppUser> AppUsers = await DataContext.AppUser.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new AppUser()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Phone = x.Phone,
            }).ToListAsync();
            
            List<LocationLog> LocationLogs = await DataContext.LocationLog.AsNoTracking()
                .Where(x => Ids.Contains(x.AppUserId))
                .Where(x => x.DeletedAt == null)
                .Select(x => new LocationLog
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    Latitude = x.Latitude,
                    Longtitude = x.Longtitude,
                    UpdateInterval = x.UpdateInterval,
                }).ToListAsync();
            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.LocationLogs = LocationLogs
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new AppUser()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Phone = x.Phone,
                SexId = x.SexId,
                Birthday = x.Birthday,
                RoleId = x.RoleId,
            }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            AppUser.Sex = await DataContext.Sex.AsNoTracking()
                .Where(x => x.Id == AppUser.SexId)
                .Select(x => new Sex
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                }).FirstOrDefaultAsync();
            AppUser.Role = await DataContext.Role.AsNoTracking()
                .Where(x => x.Id == AppUser.RoleId)
                .Select(x => new Role
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                }).FirstOrDefaultAsync();
            AppUser.LocationLogs = await DataContext.LocationLog.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new LocationLog
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    Latitude = x.Latitude,
                    Longtitude = x.Longtitude,
                    UpdateInterval = x.UpdateInterval,
                }).ToListAsync();
            AppUser.AppUserAppUserMappingAppUsers = await DataContext.AppUserAppUserMapping.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id).Where(x => x.DeletedAt == null)
                .Select(x => new AppUserAppUserMapping
                {
                    AppUserId = x.AppUserId,
                    FriendId = x.FriendId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        CreatedAt = x.AppUser.CreatedAt,
                        UpdatedAt = x.AppUser.UpdatedAt,
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                    },
                    Friend = x.Friend == null ? null : new AppUser
                    {
                        CreatedAt = x.AppUser.CreatedAt,
                        UpdatedAt = x.AppUser.UpdatedAt,
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                    },
                }).ToListAsync();
            AppUser.AppUserAppUserMappingFriends = await DataContext.AppUserAppUserMapping.AsNoTracking()
                .Where(x => x.FriendId == AppUser.Id).Where(x => x.DeletedAt == null)
                .Select(x => new AppUserAppUserMapping
                {
                    AppUserId = x.AppUserId,
                    FriendId = x.FriendId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        CreatedAt = x.AppUser.CreatedAt,
                        UpdatedAt = x.AppUser.UpdatedAt,
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                    },
                    Friend = x.Friend == null ? null : new AppUser
                    {
                        CreatedAt = x.Friend.CreatedAt,
                        UpdatedAt = x.Friend.UpdatedAt,
                        Id = x.Friend.Id,
                        Username = x.Friend.Username,
                        DisplayName = x.Friend.DisplayName,
                        Email = x.Friend.Email,
                        Phone = x.Friend.Phone,
                    }

                }).ToListAsync();
            AppUser.TrackingTargets = await DataContext.Tracking.AsNoTracking()
                .Where(x => x.TargetId == AppUser.Id).Where(x => x.DeletedAt == null)
                .Select(x => new Tracking
                {
                    Id = x.Id,
                    TrackerId = x.TrackerId,
                    TargetId = x.TargetId,
                    PlaceCheckingId = x.PlaceCheckingId,
                    PlaceId = x.PlaceId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Target = x.Target == null ? null : new AppUser
                    {
                        CreatedAt = x.Target.CreatedAt,
                        UpdatedAt = x.Target.UpdatedAt,
                        Id = x.Target.Id,
                        Username = x.Target.Username,
                        DisplayName = x.Target.DisplayName,
                        Email = x.Target.Email,
                        Phone = x.Target.Phone,
                    },
                    Tracker = x.Tracker == null ? null : new AppUser
                    {
                        CreatedAt = x.Tracker.CreatedAt,
                        UpdatedAt = x.Tracker.UpdatedAt,
                        Id = x.Tracker.Id,
                        Username = x.Tracker.Username,
                        DisplayName = x.Tracker.DisplayName,
                        Email = x.Tracker.Email,
                        Phone = x.Tracker.Phone,
                    },
                }).ToListAsync();
            AppUser.TrackingTrackers = await DataContext.Tracking.AsNoTracking()
                .Where(x => x.TrackerId == AppUser.Id).Where(x => x.DeletedAt == null)
                .Select(x => new Tracking
                {
                    Id = x.Id,
                    TrackerId = x.TrackerId,
                    TargetId = x.TargetId,
                    PlaceCheckingId = x.PlaceCheckingId,
                    PlaceId = x.PlaceId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Target = x.Target == null ? null : new AppUser
                    {
                        CreatedAt = x.Target.CreatedAt,
                        UpdatedAt = x.Target.UpdatedAt,
                        Id = x.Target.Id,
                        Username = x.Target.Username,
                        DisplayName = x.Target.DisplayName,
                        Email = x.Target.Email,
                        Phone = x.Target.Phone,
                    },
                    Tracker = x.Tracker == null ? null : new AppUser
                    {
                        CreatedAt = x.Tracker.CreatedAt,
                        UpdatedAt = x.Tracker.UpdatedAt,
                        Id = x.Tracker.Id,
                        Username = x.Tracker.Username,
                        DisplayName = x.Tracker.DisplayName,
                        Email = x.Tracker.Email,
                        Phone = x.Tracker.Phone,
                    },
                }).ToListAsync();

            return AppUser;
        }
        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.RoleId = AppUser.RoleId;
            AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.AppUser.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser.Where(x => x.Id == AppUser.Id).FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUser.Where(x => x.Id == AppUser.Id).UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = new AppUserDAO();
                AppUserDAO.Id = AppUser.Id;
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.Password = AppUser.Password;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.Birthday = AppUser.Birthday;
                AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
                AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUser
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new AppUserDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(AppUser AppUser)
        {
            List<LocationLogDAO> LocationLogDAOs = await DataContext.LocationLog
                .Where(x => x.AppUserId == AppUser.Id).ToListAsync();
            LocationLogDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (AppUser.LocationLogs != null)
            {
                foreach (LocationLog LocationLog in AppUser.LocationLogs)
                {
                    LocationLogDAO LocationLogDAO = LocationLogDAOs
                        .Where(x => x.Id == LocationLog.Id && x.Id != 0).FirstOrDefault();
                    if (LocationLogDAO == null)
                    {
                        LocationLogDAO = new LocationLogDAO();
                        LocationLogDAO.Id = LocationLog.Id;
                        LocationLogDAO.AppUserId = AppUser.Id;
                        LocationLogDAO.Latitude = LocationLog.Latitude;
                        LocationLogDAO.Longtitude = LocationLog.Longtitude;
                        LocationLogDAO.UpdateInterval = LocationLog.UpdateInterval;
                        LocationLogDAOs.Add(LocationLogDAO);
                        LocationLogDAO.CreatedAt = StaticParams.DateTimeNow;
                        LocationLogDAO.DeletedAt = null;
                    }
                    else
                    {
                        LocationLogDAO.Id = LocationLog.Id;
                        LocationLogDAO.AppUserId = AppUser.Id;
                        LocationLogDAO.Latitude = LocationLog.Latitude;
                        LocationLogDAO.Longtitude = LocationLog.Longtitude;
                        LocationLogDAO.UpdateInterval = LocationLog.UpdateInterval;
                        LocationLogDAO.DeletedAt = null;
                    }
                }
                await DataContext.LocationLog.BulkMergeAsync(LocationLogDAOs);
            }

            if (AppUser.AppUserAppUserMappingAppUsers != null)
            {
                await DataContext.AppUserAppUserMapping
                    .Where(x => x.AppUserId == AppUser.Id).DeleteFromQueryAsync();
                List<AppUserAppUserMappingDAO> AppUserAppUserMappingDAOs = AppUser.AppUserAppUserMappingAppUsers
                    .Select(x => new AppUserAppUserMappingDAO
                    {
                        AppUserId = AppUser.Id,
                        FriendId = x.FriendId,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = StaticParams.DateTimeNow,
                        DeletedAt = x.DeletedAt,
                    }).ToList();
                await DataContext.BulkMergeAsync(AppUserAppUserMappingDAOs);
            }
            
            if (AppUser.AppUserAppUserMappingFriends != null)
            {
                await DataContext.AppUserAppUserMapping
                    .Where(x => x.FriendId == AppUser.Id).DeleteFromQueryAsync();
                List<AppUserAppUserMappingDAO> AppUserAppUserMappingDAOs = AppUser.AppUserAppUserMappingFriends
                    .Select(x => new AppUserAppUserMappingDAO
                    {
                        AppUserId = x.AppUserId,
                        FriendId = AppUser.Id,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = StaticParams.DateTimeNow,
                        DeletedAt = x.DeletedAt,
                    }).ToList();
                await DataContext.BulkMergeAsync(AppUserAppUserMappingDAOs);
            }
        }
        
    }
}
