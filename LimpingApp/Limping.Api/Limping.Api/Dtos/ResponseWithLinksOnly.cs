using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;

namespace Limping.Api.Dtos
{
    public class ResponseWithLinksOnly
    {
        public List<Link> _links { get; set; }
    }
}
