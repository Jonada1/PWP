using Halcyon.HAL;
using Limping.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos
{
    public class GetUserByIdProduces: UserDto
    {
        public List<LimpingTest> _embedded { get; set; }
    }
}
