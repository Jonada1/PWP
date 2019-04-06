using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Constants;
using Limping.Api.Extensions;

namespace Limping.Api.Dtos.UserDtos.Responses
{
    public class GetUserResponse : HALResponse
    {
        public GetUserResponse(AppUser user, Link selflink = null) : base(new UserDto(user))
        {
            this.AddLinks(
                new LinkExtended("edit", $"{ControllerUrls.AppUsers}EditUser/{user.Id}", "Edit user", LinkMethods.PATCH, nameof(EditUserDto)),
                new LinkExtended("create", $"{ControllerUrls.AppUsers}CreateUser", "Create user", LinkMethods.POST, nameof(CreateUserDto)),
                new Link("delete", $"{ControllerUrls.AppUsers}/Delete/{user.Id}", "Delete user", LinkMethods.DELETE),
                new Link("getAll", $"{ControllerUrls.AppUsers}/GetAll", "Get all users", LinkMethods.GET),
                new Link("limpingTests", $"{ControllerUrls.LimpingTests}/GetForUser/{user.Id}","Get limping tests for user", LinkMethods.GET)
            );

            // Add self link if it was passed otherwise create it
            this.AddLinks(selflink ?? new Link("self", $"{ControllerUrls.AppUsers}GetById/{user.Id}", null, LinkMethods.GET));
        }
    }
}
