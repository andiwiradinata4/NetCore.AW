﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using AW.Core.DTOs;
using AW.Core.Entities.Interface;
using AW.Core.Entities;
using AW.Infrastructure.Interfaces.Services;

namespace AW.Web.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<TDbContext, T> : BaseSelectController<TDbContext, T> where TDbContext : DbContext where T : BaseEntity, IEntityStandard
    {
        public BaseController()
        {
        }

        public BaseController(IBaseService<TDbContext, T> svc) : base(svc) { }

        [HttpPost]
        public virtual IActionResult Create(T obj)
        {
            if (!base.ModelState.IsValid)
            {
                return BadRequest(base.ModelState);
            }

            MessageObject<T> messageObject = svc.Create(obj);
            if (messageObject.ProcessingStatus)
            {
                return Ok(messageObject);
            }

            return BadRequest(messageObject);
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update([FromRoute] string id, [FromBody] T obj)
        {
            if (!base.ModelState.IsValid)
            {
                return BadRequest(base.ModelState);
            }

            if (id != obj.Id)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
                defaultInterpolatedStringHandler.AppendLiteral("Route Id ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" not equal to Body Id ");
                defaultInterpolatedStringHandler.AppendFormatted(obj.Id);
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (obj == null || obj.RowVersion == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
                defaultInterpolatedStringHandler.AppendLiteral("RowVersion of Body Id ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" cannot be null. ");
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            T? data = svc.GetById(obj.Id);
            if (data == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Data ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" not found. ");
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (Convert.ToBase64String(data.RowVersion) != Convert.ToBase64String(obj.RowVersion))
            {
                MessageObject<T> messageObject = new MessageObject<T>();
                messageObject.AddMessage(MessageType.Error, "RowVersion Conflict", "The record has been changed by another user since you started editing it. Please try again.", "RowVersion");
                return Conflict(messageObject);
            }

            try
            {
                MessageObject<T> messageObject2 = svc.Update(id, obj);
                if (messageObject2.ProcessingStatus)
                {
                    return Ok(messageObject2);
                }

                return BadRequest(messageObject2);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!svc.Exists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPut("{id}/disable")]
        public virtual IActionResult Disable([FromRoute] string id, [FromBody] T obj)
        {
            if (!base.ModelState.IsValid)
            {
                return BadRequest(base.ModelState);
            }

            if (id != obj.Id)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
                defaultInterpolatedStringHandler.AppendLiteral("Route Id ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" not equal to Body Id ");
                defaultInterpolatedStringHandler.AppendFormatted(obj.Id);
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (obj == null || obj.RowVersion == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
                defaultInterpolatedStringHandler.AppendLiteral("RowVersion of Body Id ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" cannot be null. ");
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            T? data = svc.GetById(obj.Id);
            if (data == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Data ");
                defaultInterpolatedStringHandler.AppendFormatted(id);
                defaultInterpolatedStringHandler.AppendLiteral(" not found. ");
                return BadRequest(defaultInterpolatedStringHandler.ToStringAndClear());
            }

            if (Convert.ToBase64String(data.RowVersion) != Convert.ToBase64String(obj.RowVersion))
            {
                MessageObject<T> messageObject = new MessageObject<T>();
                messageObject.AddMessage(MessageType.Error, "RowVersion Conflict", "The record has been changed by another user since you started editing it. Please try again.", "RowVersion");
                return Conflict(messageObject);
            }

            try
            {
                MessageObject<T> messageObject2 = svc.Disable(id, obj);
                if (messageObject2.ProcessingStatus)
                {
                    return Ok(messageObject2);
                }

                return BadRequest(messageObject2);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!svc.Exists(id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(string id)
        {
            T? data = svc.GetById(id);
            if (data == null)
            {
                return NotFound();
            }

            MessageObject<T> messageObject = svc.Delete(data);
            if (messageObject.ProcessingStatus)
            {
                return Ok(messageObject);
            }

            return BadRequest(messageObject);
        }
    }
}