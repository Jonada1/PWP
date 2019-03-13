using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var response = new HALResponse(null)
                .AddLinks(
                    new Link("self", "/api/Root"),
                    new Link("create_user", "/api/Users/CreateUser")
                );

            return Ok(response);
        }
    }
}