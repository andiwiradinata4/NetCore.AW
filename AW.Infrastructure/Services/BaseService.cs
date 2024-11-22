using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Core.DTOs;
using AW.Core.Entities.Interface;
using AW.Infrastructure.Interfaces.Repositories;
using AW.Infrastructure.Interfaces.Services;
using AW.Infrastructure.Utils;

namespace AW.Infrastructure.Services
{
    public class BaseService<TDbContext, T> : IBaseService<TDbContext, T> where TDbContext : DbContext where T : IEntityStandard
    {
        protected string _logSource;
        protected readonly IBaseRepository<TDbContext, T> repo;
        public BaseService(IBaseRepository<TDbContext, T> repo)
        {
            this.repo = repo;
            _logSource = "BaseService<" + typeof(T).Name + ">";
        }

        public virtual ICollection<T> GetAll()
        {
            return repo.GetAll();
        }

        public virtual object GetAll(QueryObject query, bool withDisabled)
        {
            return repo.GetAll(query, withDisabled);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await repo.GetAllAsync();
        }

        //public virtual IQueryable GetByODataQuery(ODataQueryOptions<T> queryOptions)
        //{
        //    return repo.GetByODataQuery(queryOptions);
        //}

        public virtual T? GetById(string Id)
        {
            return repo.GetById(Id);
        }

        public virtual async Task<T?> GetByIDAsync(string Id)
        {
            return await repo.GetByIDAsync(Id);
        }

        public virtual bool Exists(string id)
        {
            return GetById(id) != null;
        }

        public virtual bool ExistsInDb(Func<T, bool> predicate)
        {
            return repo.ExistsInDb(predicate);
        }

        public virtual bool ExistsInDbWithDisabledRecord(Func<T, bool> predicate)
        {
            return repo.ExistsInDbWithDisabledRecord(predicate);
        }

        public virtual MessageObject<T> Create(T entity)
        {
            MessageObject<T> messageObject = ValidateCreate(entity);
            try
            {
                if (messageObject.ProcessingStatus)
                {
                    GetNewID(entity);
                    BeforeCreate(entity);
                    messageObject.Data = repo.Create(entity);
                    AfterCreate(entity);
                }
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public virtual async Task<MessageObject<T>> CreateAsync(T entity)
        {
            MessageObject<T> messageObject = ValidateCreate(entity);
            try
            {
                if (messageObject.ProcessingStatus)
                {
                    GetNewID(entity);
                    BeforeCreate(entity);
                    messageObject.Data = await repo.CreateAsync(entity);
                    AfterCreate(entity);
                }
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public virtual MessageObject<T> Update(string id, T entity)
        {
            MessageObject<T> messageObject = ValidateUpdate(entity);
            try
            {
                if (messageObject.ProcessingStatus)
                {
                    BeforeUpdate(entity);
                    messageObject.Data = repo.Update(id, entity);
                    AfterCreate(entity);
                }
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public virtual async Task<MessageObject<T>> UpdateAsync(string id, T entity)
        {
            MessageObject<T> messageObject = ValidateUpdate(entity);
            try
            {
                if (messageObject.ProcessingStatus)
                {
                    BeforeUpdate(entity);
                    messageObject.Data = await repo.UpdateAsync(id, entity);
                    AfterCreate(entity);
                }
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public virtual MessageObject<T> Disable(string id)
        {
            T? entity = repo.GetById(id);
            return Disable(id, entity!);
        }

        public virtual MessageObject<T> Disable(string id, T entity)
        {
            MessageObject<T> messageObject = new MessageObject<T>();
            try
            {
                if (entity != null)
                {
                    if (messageObject.ProcessingStatus)
                    {
                        BeforeDisable(entity);
                        messageObject.Data = repo.Disable(entity);
                        AfterDisable(entity);
                    }
                }
                else messageObject.AddMessage(MessageType.Error, "Data Not Found", "Data not found.", "undefined");
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public virtual MessageObject<T> Delete(string id)
        {
            T? entity = repo.GetById(id);
            return Delete(entity!);
        }

        public virtual MessageObject<T> Delete(T entity)
        {
            MessageObject<T> messageObject = new MessageObject<T>();
            try
            {
                if (messageObject.ProcessingStatus)
                {
                    messageObject = ValidateRemove(entity!);
                    BeforeRemove(entity);
                    messageObject.Data = repo.Delete(entity);
                    AfterRemove(entity);
                }
            }
            catch (Exception ex)
            {
                /// Log Error
                Console.WriteLine(ex.Message);
                messageObject.AddException(ex);
            }
            return messageObject;
        }

        public object GetColumnSet()
        {
            return repo.GetColumnSet();
        }

        public virtual T GetNewID(T entity)
        {
            if (Int64.TryParse(entity.Id, out var newId))
            {
                entity.Id = (repo.Count() + 1).ToString();
            }
            else
            {
                entity.Id = Helper.GetDateTimeFormat_yyyyMMddHHmmssffff();
            }
            return entity;
        }

        protected virtual MessageObject<T> ValidateCreate(T entity)
        {
            return new MessageObject<T>(entity);
        }

        protected virtual MessageObject<T> ValidateUpdate(T entity)
        {
            return new MessageObject<T>(entity);
        }

        protected virtual MessageObject<T> ValidateDisable(T entity)
        {
            return new MessageObject<T>(entity);
        }

        protected virtual MessageObject<T> ValidateRemove(T entity)
        {
            return new MessageObject<T>(entity);
        }

        //protected virtual T GetNewTransactionNumber(T entity, object[] initial)
        //{
        //    if (Int64.TryParse(entity.Id, out var newId))
        //    {
        //        entity.Id = (repo.Count() + 1).ToString();
        //    }
        //    else
        //    {
        //        if (initial.Count() == 0)
        //        {
        //            entity.Id = Guid.NewGuid().ToString();
        //        }
        //        else
        //        {
        //            var allAutoNumber = _autoNumberRepo.GetByConditionAsQueryable(e => e.Initial == initial.First().ToString());
        //            string format = "";
        //            if (allAutoNumber.Count() == 0)
        //            {
        //                format = initial.First().ToString() ?? "" + "{1:yyyyMMdd}-{2}-{3}-{0:0000}";
        //                _autoNumberRepo.Create(new AutoNumber()
        //                {
        //                    Initial = initial.First().ToString() ?? "",
        //                    Format = format
        //                });
        //                _autoNumberRepo.SaveChanges();
        //            }
        //            else
        //            {
        //                format = allAutoNumber.First().Format;
        //            }
        //            entity.Id = string.Format(format, initial);
        //        }
        //    }
        //    return entity;
        //}

        protected virtual T BeforeCreate(T entity)
        {
            return entity;
        }

        protected virtual T AfterCreate(T entity)
        {
            return entity;
        }

        protected virtual T BeforeUpdate(T entity)
        {
            return entity;
        }

        protected virtual T AfterUpdate(T entity)
        {
            return entity;
        }

        protected virtual T BeforeDisable(T entity)
        {
            return entity;
        }

        protected virtual T AfterDisable(T entity)
        {
            return entity;
        }

        protected virtual T BeforeRemove(T entity)
        {
            return entity;
        }

        protected virtual T AfterRemove(T entity)
        {
            return entity;
        }
    }
}
