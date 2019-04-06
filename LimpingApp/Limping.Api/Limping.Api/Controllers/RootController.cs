using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Dtos;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseWithLinksOnly))]
        public IActionResult Get()
        {
            var response = new HALResponse(null)
                .AddLinks(
                    new Link("self", "/api/Root"),
                    new Link("getAllUsers", $"{ControllerUrls.AppUsers}/GetAll", "Get all users", LinkMethods.GET),
                    new LinkExtended("createUser", $"{ControllerUrls.AppUsers}CreateUser", "Create user", LinkMethods.POST, nameof(CreateUserDto))
                );
            return Ok(response);
        }
    }
}