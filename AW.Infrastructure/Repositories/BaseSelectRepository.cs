using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using AW.Core.Entities.Interface;
using AW.Core.Entities;
using AW.Infrastructure.Interfaces.Repositories;
using AW.Core.DTOs;
using AW.Core.Extensions;

namespace AW.Infrastructure.Repositories
{
    public class BaseSelectRepository<TDbContext, T> : IBaseSelectRepository<TDbContext, T> where TDbContext : DbContext where T : BaseEntity, IEntityStandard
    {
        protected TDbContext context;
        protected DbSet<T> dbSet;

        public BaseSelectRepository(TDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual bool ExistsInDb(Func<T, bool> predicate)
        {
            return dbSet.Where((T c) => !c.Disabled).Any(predicate);
        }

        public virtual bool ExistsInDbWithDisabledRecord(Func<T, bool> predicate)
        {
            return dbSet.IgnoreQueryFilters().Any(predicate);
        }

        public virtual ICollection<T> GetAll()
        {
            return (from s in dbSet.AsNoTracking()
                    where !s.Disabled
                    select s into m
                    orderby m.CreatedDate descending
                    select m).ToList();
        }

        public virtual object GetAll(QueryObject query, bool withDisabled)
        {
            //string companyId = ComLoc.CompanyId;
            //string programId = ComLoc.ProgramId;

            IQueryable<T> queryable;
            if (withDisabled)
            {
                queryable = dbSet.SetQuery(query);
            }
            else
            {
                queryable = dbSet.Where(e => e.Disabled == false).SetQuery(query);
            }

            int count = queryable.Count();
            int totalPage = 0;

            if (queryable.Count() == 0)
            {
                totalPage = 0;
            }
            else if (query.PageSize == 0 || query.Page == 0 || count < query.PageSize) { totalPage = 1; }
            if (queryable.Count() > 0 && query.PageSize > 0 && count >= query.PageSize) totalPage = count / query.PageSize;

            // Apply Pagination
            if (query.Page >= 0) queryable = queryable.Skip((query.Page) * query.PageSize).Take(query.PageSize);

            // Apply Includes
            var allIncludes = query.Includes.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
            allIncludes.ForEach(e =>
            {
                queryable = queryable.Include(e);
            });


            if (!string.IsNullOrEmpty(query.Columns)) return new { TotalCount = count, TotalPage = totalPage, DataSet = queryable.Select("new(" + query.Columns + ")") };

            return new { TotalCount = count, TotalPage = totalPage, DataSet = queryable };
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await (from s in dbSet.AsNoTracking()
                          where !s.Disabled
                          select s into m
                          orderby m.CreatedDate descending
                          select m).ToListAsync();
        }

        public virtual IQueryable<T> GetByConditionAsQueryable(Expression<Func<T, bool>> predicate)
        {
            return from c in dbSet.Where(predicate)
                   where !c.Disabled
                   select c;
        }

        public virtual IQueryable<T> GetByConditionAsQueryableWithDisabledRecord(Expression<Func<T, bool>> predicate)
        {
            return dbSet.IgnoreQueryFilters().Where(predicate);
        }

        public virtual T? GetById(string Id)
        {
            return dbSet.Find(Id);
        }

        public virtual async Task<T?> GetByIDAsync(string Id)
        {
            return await dbSet.FindAsync(Id);
        }

        public virtual object? GetByIdWithQueryObject(string Id, QueryObject query)
        {
            IQueryable<T> queryable = dbSet.Where(e => e.Id == Id).SetQuery(query);

            // Apply Includes
            var allIncludes = query.Includes.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
            allIncludes.ForEach(e =>
            {
                queryable = queryable.Include(e);
            });

            if (queryable.Count() == 0) return null;

            if (!string.IsNullOrEmpty(query.Columns)) return queryable.Select("new(" + query.Columns + ")").First();

            return queryable.First();
        }

        //public IQueryable GetByODataQuery(ODataQueryOptions<T> queryOptions)
        //{
        //    return queryOptions.ApplyTo(dbSet.AsNoTracking());
        //}

        public virtual int Count()
        {
            return dbSet.Count();
        }

        public virtual int CountByCondition(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate).Count();
        }

        public virtual async Task<int> CountByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).CountAsync();
        }

        public object GetColumnSet()
        {
            string? fullName = typeof(T).FullName;

            var entityType = context.Model.FindEntityType(fullName ?? "");

            var columns = entityType?.GetProperties().Where(e => e.ClrType.Name.ToLower() == "string");
            ICollection<ColumnSet> allColumns = new List<ColumnSet>();

            if (columns == null) return allColumns;

            allColumns = columns.Select((e) => new ColumnSet() { Name = e.Name, Type = e.GetColumnType(), Size = e.GetMaxLength() ?? 0 }).ToList();
            return allColumns;
        }

    }
}
