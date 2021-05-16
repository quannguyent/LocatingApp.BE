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
    public interface ICheckingStatusRepository
    {
        Task<int> Count(CheckingStatusFilter CheckingStatusFilter);
        Task<List<CheckingStatus>> List(CheckingStatusFilter CheckingStatusFilter);
        Task<List<CheckingStatus>> List(List<long> Ids);
        Task<CheckingStatus> Get(long Id);
        Task<bool> Create(CheckingStatus CheckingStatus);
        Task<bool> Update(CheckingStatus CheckingStatus);
        Task<bool> Delete(CheckingStatus CheckingStatus);
        Task<bool> BulkMerge(List<CheckingStatus> CheckingStatuses);
        Task<bool> BulkDelete(List<CheckingStatus> CheckingStatuses);
    }
    public class CheckingStatusRepository : ICheckingStatusRepository
    {
        private DataContext DataContext;
        public CheckingStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<CheckingStatusDAO> DynamicFilter(IQueryable<CheckingStatusDAO> query, CheckingStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null && filter.Code.HasValue)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<CheckingStatusDAO> OrFilter(IQueryable<CheckingStatusDAO> query, CheckingStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<CheckingStatusDAO> initQuery = query.Where(q => false);
            foreach (CheckingStatusFilter CheckingStatusFilter in filter.OrFilter)
            {
                IQueryable<CheckingStatusDAO> queryable = query;
                if (CheckingStatusFilter.Id != null && CheckingStatusFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (CheckingStatusFilter.Code != null && CheckingStatusFilter.Code.HasValue)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (CheckingStatusFilter.Name != null && CheckingStatusFilter.Name.HasValue)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<CheckingStatusDAO> DynamicOrder(IQueryable<CheckingStatusDAO> query, CheckingStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case CheckingStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case CheckingStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case CheckingStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case CheckingStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case CheckingStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case CheckingStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<CheckingStatus>> DynamicSelect(IQueryable<CheckingStatusDAO> query, CheckingStatusFilter filter)
        {
            List<CheckingStatus> CheckingStatuses = await query.Select(q => new CheckingStatus()
            {
                Id = filter.Selects.Contains(CheckingStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(CheckingStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(CheckingStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return CheckingStatuses;
        }

        public async Task<int> Count(CheckingStatusFilter filter)
        {
            IQueryable<CheckingStatusDAO> CheckingStatuses = DataContext.CheckingStatus.AsNoTracking();
            CheckingStatuses = DynamicFilter(CheckingStatuses, filter);
            return await CheckingStatuses.CountAsync();
        }

        public async Task<List<CheckingStatus>> List(CheckingStatusFilter filter)
        {
            if (filter == null) return new List<CheckingStatus>();
            IQueryable<CheckingStatusDAO> CheckingStatusDAOs = DataContext.CheckingStatus.AsNoTracking();
            CheckingStatusDAOs = DynamicFilter(CheckingStatusDAOs, filter);
            CheckingStatusDAOs = DynamicOrder(CheckingStatusDAOs, filter);
            List<CheckingStatus> CheckingStatuses = await DynamicSelect(CheckingStatusDAOs, filter);
            return CheckingStatuses;
        }

        public async Task<List<CheckingStatus>> List(List<long> Ids)
        {
            List<CheckingStatus> CheckingStatuses = await DataContext.CheckingStatus.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new CheckingStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return CheckingStatuses;
        }

        public async Task<CheckingStatus> Get(long Id)
        {
            CheckingStatus CheckingStatus = await DataContext.CheckingStatus.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new CheckingStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            if (CheckingStatus == null)
                return null;

            return CheckingStatus;
        }
        public async Task<bool> Create(CheckingStatus CheckingStatus)
        {
            CheckingStatusDAO CheckingStatusDAO = new CheckingStatusDAO();
            CheckingStatusDAO.Id = CheckingStatus.Id;
            CheckingStatusDAO.Code = CheckingStatus.Code;
            CheckingStatusDAO.Name = CheckingStatus.Name;
            DataContext.CheckingStatus.Add(CheckingStatusDAO);
            await DataContext.SaveChangesAsync();
            CheckingStatus.Id = CheckingStatusDAO.Id;
            await SaveReference(CheckingStatus);
            return true;
        }

        public async Task<bool> Update(CheckingStatus CheckingStatus)
        {
            CheckingStatusDAO CheckingStatusDAO = DataContext.CheckingStatus.Where(x => x.Id == CheckingStatus.Id).FirstOrDefault();
            if (CheckingStatusDAO == null)
                return false;
            CheckingStatusDAO.Id = CheckingStatus.Id;
            CheckingStatusDAO.Code = CheckingStatus.Code;
            CheckingStatusDAO.Name = CheckingStatus.Name;
            await DataContext.SaveChangesAsync();
            await SaveReference(CheckingStatus);
            return true;
        }

        public async Task<bool> Delete(CheckingStatus CheckingStatus)
        {
            await DataContext.CheckingStatus.Where(x => x.Id == CheckingStatus.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<CheckingStatus> CheckingStatuses)
        {
            List<CheckingStatusDAO> CheckingStatusDAOs = new List<CheckingStatusDAO>();
            foreach (CheckingStatus CheckingStatus in CheckingStatuses)
            {
                CheckingStatusDAO CheckingStatusDAO = new CheckingStatusDAO();
                CheckingStatusDAO.Id = CheckingStatus.Id;
                CheckingStatusDAO.Code = CheckingStatus.Code;
                CheckingStatusDAO.Name = CheckingStatus.Name;
                CheckingStatusDAOs.Add(CheckingStatusDAO);
            }
            await DataContext.BulkMergeAsync(CheckingStatusDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<CheckingStatus> CheckingStatuses)
        {
            List<long> Ids = CheckingStatuses.Select(x => x.Id).ToList();
            await DataContext.CheckingStatus
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(CheckingStatus CheckingStatus)
        {
        }
        
    }
}
