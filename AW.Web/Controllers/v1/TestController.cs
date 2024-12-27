using AW.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AW.Web.Controllers.v1
{
    [Route("api/v1/test")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [Authorize]
        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            return Ok();
        }

    }
}
