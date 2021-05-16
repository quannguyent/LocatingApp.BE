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
    public interface IPlaceGroupRepository
    {
        Task<int> Count(PlaceGroupFilter PlaceGroupFilter);
        Task<List<PlaceGroup>> List(PlaceGroupFilter PlaceGroupFilter);
        Task<List<PlaceGroup>> List(List<long> Ids);
        Task<PlaceGroup> Get(long Id);
        Task<bool> Create(PlaceGroup PlaceGroup);
        Task<bool> Update(PlaceGroup PlaceGroup);
        Task<bool> Delete(PlaceGroup PlaceGroup);
        Task<bool> BulkMerge(List<PlaceGroup> PlaceGroups);
        Task<bool> BulkDelete(List<PlaceGroup> PlaceGroups);
    }
    public class PlaceGroupRepository : IPlaceGroupRepository
    {
        private DataContext DataContext;
        public PlaceGroupRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PlaceGroupDAO> DynamicFilter(IQueryable<PlaceGroupDAO> query, PlaceGroupFilter filter)
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
            if (filter.ParentId != null && filter.ParentId.HasValue)
                query = query.Where(q => q.ParentId.HasValue).Where(q => q.ParentId.Value, filter.ParentId);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PlaceGroupDAO> OrFilter(IQueryable<PlaceGroupDAO> query, PlaceGroupFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PlaceGroupDAO> initQuery = query.Where(q => false);
            foreach (PlaceGroupFilter PlaceGroupFilter in filter.OrFilter)
            {
                IQueryable<PlaceGroupDAO> queryable = query;
                if (PlaceGroupFilter.Id != null && PlaceGroupFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (PlaceGroupFilter.ParentId != null && PlaceGroupFilter.ParentId.HasValue)
                    queryable = queryable.Where(q => q.ParentId.HasValue).Where(q => q.ParentId.Value, filter.ParentId);
                if (PlaceGroupFilter.Name != null && PlaceGroupFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (PlaceGroupFilter.Code != null && PlaceGroupFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PlaceGroupDAO> DynamicOrder(IQueryable<PlaceGroupDAO> query, PlaceGroupFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PlaceGroupOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PlaceGroupOrder.Parent:
                            query = query.OrderBy(q => q.ParentId);
                            break;
                        case PlaceGroupOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PlaceGroupOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PlaceGroupOrder.Used:
                            query = query.OrderBy(q => q.Used);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PlaceGroupOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PlaceGroupOrder.Parent:
                            query = query.OrderByDescending(q => q.ParentId);
                            break;
                        case PlaceGroupOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PlaceGroupOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PlaceGroupOrder.Used:
                            query = query.OrderByDescending(q => q.Used);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PlaceGroup>> DynamicSelect(IQueryable<PlaceGroupDAO> query, PlaceGroupFilter filter)
        {
            List<PlaceGroup> PlaceGroups = await query.Select(q => new PlaceGroup()
            {
                Id = filter.Selects.Contains(PlaceGroupSelect.Id) ? q.Id : default(long),
                ParentId = filter.Selects.Contains(PlaceGroupSelect.Parent) ? q.ParentId : default(long?),
                Name = filter.Selects.Contains(PlaceGroupSelect.Name) ? q.Name : default(string),
                Code = filter.Selects.Contains(PlaceGroupSelect.Code) ? q.Code : default(string),
                Used = filter.Selects.Contains(PlaceGroupSelect.Used) ? q.Used : default(bool),
                Parent = filter.Selects.Contains(PlaceGroupSelect.Parent) && q.Parent != null ? new PlaceGroup
                {
                    Id = q.Parent.Id,
                    ParentId = q.Parent.ParentId,
                    Name = q.Parent.Name,
                    Code = q.Parent.Code,
                    Used = q.Parent.Used,
                } : null,
            }).ToListAsync();
            return PlaceGroups;
        }

        public async Task<int> Count(PlaceGroupFilter filter)
        {
            IQueryable<PlaceGroupDAO> PlaceGroups = DataContext.PlaceGroup.AsNoTracking();
            PlaceGroups = DynamicFilter(PlaceGroups, filter);
            return await PlaceGroups.CountAsync();
        }

        public async Task<List<PlaceGroup>> List(PlaceGroupFilter filter)
        {
            if (filter == null) return new List<PlaceGroup>();
            IQueryable<PlaceGroupDAO> PlaceGroupDAOs = DataContext.PlaceGroup.AsNoTracking();
            PlaceGroupDAOs = DynamicFilter(PlaceGroupDAOs, filter);
            PlaceGroupDAOs = DynamicOrder(PlaceGroupDAOs, filter);
            List<PlaceGroup> PlaceGroups = await DynamicSelect(PlaceGroupDAOs, filter);
            return PlaceGroups;
        }

        public async Task<List<PlaceGroup>> List(List<long> Ids)
        {
            List<PlaceGroup> PlaceGroups = await DataContext.PlaceGroup.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new PlaceGroup()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                Code = x.Code,
                Used = x.Used,
                Parent = x.Parent == null ? null : new PlaceGroup
                {
                    Id = x.Parent.Id,
                    ParentId = x.Parent.ParentId,
                    Name = x.Parent.Name,
                    Code = x.Parent.Code,
                    Used = x.Parent.Used,
                },
            }).ToListAsync();
            

            return PlaceGroups;
        }

        public async Task<PlaceGroup> Get(long Id)
        {
            PlaceGroup PlaceGroup = await DataContext.PlaceGroup.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new PlaceGroup()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                Code = x.Code,
                Used = x.Used,
                Parent = x.Parent == null ? null : new PlaceGroup
                {
                    Id = x.Parent.Id,
                    ParentId = x.Parent.ParentId,
                    Name = x.Parent.Name,
                    Code = x.Parent.Code,
                    Used = x.Parent.Used,
                },
            }).FirstOrDefaultAsync();

            if (PlaceGroup == null)
                return null;

            return PlaceGroup;
        }
        public async Task<bool> Create(PlaceGroup PlaceGroup)
        {
            PlaceGroupDAO PlaceGroupDAO = new PlaceGroupDAO();
            PlaceGroupDAO.Id = PlaceGroup.Id;
            PlaceGroupDAO.ParentId = PlaceGroup.ParentId;
            PlaceGroupDAO.Name = PlaceGroup.Name;
            PlaceGroupDAO.Code = PlaceGroup.Code;
            PlaceGroupDAO.Used = PlaceGroup.Used;
            PlaceGroupDAO.Path = "";
            PlaceGroupDAO.Level = 1;
            PlaceGroupDAO.CreatedAt = StaticParams.DateTimeNow;
            PlaceGroupDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.PlaceGroup.Add(PlaceGroupDAO);
            await DataContext.SaveChangesAsync();
            PlaceGroup.Id = PlaceGroupDAO.Id;
            await SaveReference(PlaceGroup);
            await BuildPath();
            return true;
        }

        public async Task<bool> Update(PlaceGroup PlaceGroup)
        {
            PlaceGroupDAO PlaceGroupDAO = DataContext.PlaceGroup.Where(x => x.Id == PlaceGroup.Id).FirstOrDefault();
            if (PlaceGroupDAO == null)
                return false;
            PlaceGroupDAO.Id = PlaceGroup.Id;
            PlaceGroupDAO.ParentId = PlaceGroup.ParentId;
            PlaceGroupDAO.Name = PlaceGroup.Name;
            PlaceGroupDAO.Code = PlaceGroup.Code;
            PlaceGroupDAO.Used = PlaceGroup.Used;
            PlaceGroupDAO.Path = "";
            PlaceGroupDAO.Level = 1;
            PlaceGroupDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(PlaceGroup);
            await BuildPath();
            return true;
        }

        public async Task<bool> Delete(PlaceGroup PlaceGroup)
        {
            PlaceGroupDAO PlaceGroupDAO = await DataContext.PlaceGroup.Where(x => x.Id == PlaceGroup.Id).FirstOrDefaultAsync();
            await DataContext.PlaceGroup.Where(x => x.Path.StartsWith(PlaceGroupDAO.Id + ".")).UpdateFromQueryAsync(x => new PlaceGroupDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await DataContext.PlaceGroup.Where(x => x.Id == PlaceGroup.Id).UpdateFromQueryAsync(x => new PlaceGroupDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PlaceGroup> PlaceGroups)
        {
            List<PlaceGroupDAO> PlaceGroupDAOs = new List<PlaceGroupDAO>();
            foreach (PlaceGroup PlaceGroup in PlaceGroups)
            {
                PlaceGroupDAO PlaceGroupDAO = new PlaceGroupDAO();
                PlaceGroupDAO.Id = PlaceGroup.Id;
                PlaceGroupDAO.ParentId = PlaceGroup.ParentId;
                PlaceGroupDAO.Name = PlaceGroup.Name;
                PlaceGroupDAO.Code = PlaceGroup.Code;
                PlaceGroupDAO.Used = PlaceGroup.Used;
                PlaceGroupDAO.CreatedAt = StaticParams.DateTimeNow;
                PlaceGroupDAO.UpdatedAt = StaticParams.DateTimeNow;
                PlaceGroupDAOs.Add(PlaceGroupDAO);
            }
            await DataContext.BulkMergeAsync(PlaceGroupDAOs);
            await BuildPath();
            return true;
        }

        public async Task<bool> BulkDelete(List<PlaceGroup> PlaceGroups)
        {
            List<long> Ids = PlaceGroups.Select(x => x.Id).ToList();
            await DataContext.PlaceGroup
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PlaceGroupDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            await BuildPath();
            return true;
        }

        private async Task SaveReference(PlaceGroup PlaceGroup)
        {
        }
        
        private async Task BuildPath()
        {
            List<PlaceGroupDAO> PlaceGroupDAOs = await DataContext.PlaceGroup
                .Where(x => x.DeletedAt == null)
                .AsNoTracking().ToListAsync();
            Queue<PlaceGroupDAO> queue = new Queue<PlaceGroupDAO>();
            PlaceGroupDAOs.ForEach(x =>
            {
                if (!x.ParentId.HasValue)
                {
                    x.Path = x.Id + ".";
                    x.Level = 1;
                    queue.Enqueue(x);
                }
            });
            while(queue.Count > 0)
            {
                PlaceGroupDAO Parent = queue.Dequeue();
                foreach (PlaceGroupDAO PlaceGroupDAO in PlaceGroupDAOs)
                {
                    if (PlaceGroupDAO.ParentId == Parent.Id)
                    {
                        PlaceGroupDAO.Path = Parent.Path + PlaceGroupDAO.Id + ".";
                        PlaceGroupDAO.Level = Parent.Level + 1;
                        queue.Enqueue(PlaceGroupDAO);
                    }
                }
            }
            await DataContext.BulkMergeAsync(PlaceGroupDAOs);
        }
    }
}
