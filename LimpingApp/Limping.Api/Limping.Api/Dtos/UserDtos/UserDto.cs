using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(AppUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
