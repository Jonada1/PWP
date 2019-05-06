using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Dtos;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Extensions;
using Limping.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Limping.Api.Controllers
{
    /// <summary>
    /// The root controller of the requests
    /// </summary>
    [Route("api/[controller]")]
    public class RootController : LimpingControllerBase
    {
        /// <summary>
        /// Get's all the links you can navigate to from the root
        /// </summary>
        /// <returns>HAL Response with links</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseWithLinksOnly))]
        public IActionResult Get()
        {
            var response = new HALResponse(null)
                .AddLinks(
                    new Link("self", "/api/Root"),
                    LinkGenerator.Users.GetAll(),
                    LinkGenerator.Users.Create()
                );
            return Ok(response);
        }
    }
}