using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AW.Core.DTOs;
using AW.Core.Entities.Interface;
using AW.Core.Entities;
using AW.Infrastructure.Interfaces.Services;

namespace AW.Web.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseSelectController<TDbContext, T> : ControllerBase where TDbContext : DbContext where T : BaseEntity, IEntityStandard
    {
        protected IBaseService<TDbContext, T> svc;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BaseSelectController()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public BaseSelectController(IBaseService<TDbContext, T> svc)
        {
            this.svc = svc;
        }

        //[HttpGet]
        //[EnableQuery]
        //public virtual IActionResult Get(ODataQueryOptions<T> queryOptions)
        //{
        //    return Ok(svc.GetByODataQuery(queryOptions));
        //}

        [HttpGet]
        public async virtual Task<IActionResult> Get()
        {
            return Ok(await svc.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async virtual Task<IActionResult> Get(string id)
        {
            T? data = await svc.GetByIDAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpGet("columns")]
        public virtual IActionResult GetColumns()
        {
            var data = svc.GetColumnSet();
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost("page")]
        public virtual IActionResult PostPageList([FromBody] QueryObject query)
        {
            return Ok(svc.GetAll(query, false));
        }

        [HttpPost("page-with-disabled")]
        public virtual IActionResult PostPageListWithDisabledRecord([FromBody] QueryObject query)
        {
            return Ok(svc.GetAll(query, true));
        }
    }
}
