using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Extensions;
using Limping.Api.Models;
using Limping.Api.Utils;

namespace Limping.Api.Dtos.UserDtos.Responses
{
    public class GetAllUsersResponse: HALResponse
    {
        public GetAllUsersResponse(List<AppUser> users): base(null)
        {
            var userResponses = users.Select(user => new GetUserResponse(user));
            this.AddEmbeddedCollection("users", userResponses);

            this.AddLinks(
                LinkGenerator.Users.GetAll("self"),
                LinkGenerator.Users.Create()
            );
        }
    }
}
