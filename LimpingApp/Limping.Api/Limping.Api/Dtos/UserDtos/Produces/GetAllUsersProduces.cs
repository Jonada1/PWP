using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Dtos.UserDtos.Produces
{
    public class GetAllUsersProduces
    {
        public List<GetUserByIdProduces> _embedded { get; set; }
    }
}
