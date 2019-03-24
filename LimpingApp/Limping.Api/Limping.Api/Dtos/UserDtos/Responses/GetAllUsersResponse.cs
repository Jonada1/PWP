using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Models;

namespace Limping.Api.Dtos.UserDtos.Responses
{
    public class GetAllUsersResponse: HALResponse
    {
        public GetAllUsersResponse(List<AppUser> users, List<Link> links = null): base(null)
        {
            var userResponses = users.Select(user => new GetUserResponse(user));
            this.AddEmbeddedCollection("users", userResponses);
            if (links == null)
            {
                this.AddLinks(new Link("self", $"/api/Users/GetAllUsers"));
            }
            else
            {
                this.AddLinks(links);
            }
        }
    }
}
