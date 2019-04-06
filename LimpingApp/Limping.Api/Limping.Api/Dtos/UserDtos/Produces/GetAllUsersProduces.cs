using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;

namespace Limping.Api.Dtos.UserDtos.Produces
{
    public class GetAllUsersProduces
    {
        public List<Link> _links { get; set; }
        public List<GetUserByIdProduces> _embedded { get; set; }
    }
}
