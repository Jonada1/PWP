using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos.Responses
{
    public class GetUserResponse : HALResponse
    {
        public GetUserResponse(AppUser user, List<Link> links = null): base(new UserDto(user))
        {
            if (links == null)
            {
                this.AddLinks(
                    new Link("self", $"/api/Users/GetById/{user.Id}", null, "GET"),
                    new Link("limpingTests", $"/api/LimpingTests/GetForUser/{user.Id}", null, "GET")
                );
            }
            else
            {
                this.AddLinks(links);
            }
        }
    }
}
