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
                    new Link("self", $"/api/Users/GetById/{user.Id}"),
                    new Link("limpingTests", $"/api/LimpingTests/GetForUser/{user.Id}")
                );
            }
            else
            {
                this.AddLinks(links);
            }
        }
    }
}
