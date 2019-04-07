using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos
{
    public class EditUserDto
    {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
