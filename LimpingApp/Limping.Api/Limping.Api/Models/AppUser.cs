using System;
using System.Collections.Generic;

namespace Limping.Api.Models
{
    public class AppUser
    {
        public string Id { get; set; }
        public virtual string UserName { get; set; }
        public string Email { get; set; }
        public virtual List<LimpingTest> LimpingTests { get; set; }
    }
}
