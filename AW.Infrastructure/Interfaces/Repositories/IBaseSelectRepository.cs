using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AW.Core.DTOs;
using AW.Core.Entities.Interface;

namespace AW.Infrastructure.Interfaces.Repositories
{
    public interface IBaseSelectRepository<TDbContext, T> where TDbContext : DbContext where T : IEntityStandard
    {
        ICollection<T> GetAll();
		MessageGetList<T> GetAll(QueryObject query, bool withDisabled);
        Task<List<T>> GetAllAsync();
        T? GetById(string Id);
        Task<T?> GetByIDAsync(string Id);
        object? GetByIdWithQueryObject(string Id, QueryObject query);
        //IQueryable GetByODataQuery(ODataQueryOptions<T> queryOptions);
        IQueryable<T> GetByConditionAsQueryable(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetByConditionAsQueryableWithDisabledRecord(Expression<Func<T, bool>> predicate);
        bool ExistsInDb(Func<T, bool> predicate);
        int Count();
        int CountByCondition(Expression<Func<T, bool>> predicate);
        Task<int> CountByConditionAsync(Expression<Func<T, bool>> predicate);
        object GetColumnSet();
        bool ExistsInDbWithDisabledRecord(Func<T, bool> predicate);
    }
}