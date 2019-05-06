using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Constants;
using Limping.Api.Extensions;
using Limping.Api.Utils;

namespace Limping.Api.Dtos.UserDtos.Responses
{
    public class GetUserResponse : HALResponse
    {
        public GetUserResponse(AppUser user, Link selflink = null) : base(new UserDto(user))
        {
            this.AddLinks(
                LinkGenerator.Users.Edit(user.Id),
                LinkGenerator.Users.Create(),
                LinkGenerator.Users.Delete(user.Id),
                LinkGenerator.LimpingTests.GetForUser(user.Id)
            );

            // Add self link if it was passed otherwise create it
            this.AddLinks(selflink ?? LinkGenerator.Users.GetSingle(user.Id, "self"));
        }
    }
}
