using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class AppUser: IdentityUser
    {
        public virtual List<LimpingTest> LimpingTests { get; set; }
    }
}
