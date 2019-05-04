using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Controllers
{
    [Produces("application/hal+json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiController]
    public class LimpingControllerBase: ControllerBase
    {
    }
}
