using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Core.DTOs;
using AW.Core.Entities.Interface;

namespace AW.Infrastructure.Interfaces.Services
{
    public interface IBaseService<TDbContext, T> where TDbContext : DbContext where T : IEntityStandard
    {
        ICollection<T> GetAll();
        object GetAll(QueryObject query, bool withDisabled);
        Task<List<T>> GetAllAsync();
        //IQueryable GetByODataQuery(ODataQueryOptions<T> queryOptions);
        T? GetById(string Id);
        Task<T?> GetByIDAsync(string Id);
        T? GetByIdWithQueryObject(string Id, QueryObject query);
        bool Exists(string id);
        bool ExistsInDb(Func<T, bool> predicate);
        bool ExistsInDbWithDisabledRecord(Func<T, bool> predicate);
        MessageObject<T> Create(T entity, bool useTransaction = false);
        Task<MessageObject<T>> CreateAsync(T entity, bool useTransaction = false);
        MessageObject<T> Update(string id, T entity, bool useTransaction = false);
        Task<MessageObject<T>> UpdateAsync(string id, T entity, bool useTransaction = false);
        MessageObject<T> Disable(string id, bool useTransaction = false);
        MessageObject<T> Disable(string id, T entity, bool useTransaction = false);
        MessageObject<T> Delete(string id, bool useTransaction = false);
        MessageObject<T> Delete(T entity, bool useTransaction = false);
        object GetColumnSet();
        T GetNewID(T entity);
    }
}
