using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;

namespace Limping.Api.Extensions
{
    public class LinkExtended : Link
    {
        public LinkExtended(string rel, string href, string title = null, string method = null, string profile = null) :
            base(rel, href, title, method)
        {
            Profile = profile;
        }
    }
}
