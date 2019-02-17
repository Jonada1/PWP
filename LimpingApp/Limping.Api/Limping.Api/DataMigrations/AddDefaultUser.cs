using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.DataMigrations
{
    public class AddDefaultUser: BaseDataMigration
    {
        private readonly LimpingDbContext _context;

        public AddDefaultUser(LimpingDbContext context)
        {
            _context = context;
        }

        public override async Task Apply()
        {
            var defaultUser = new AppUser
            {
                Email = "jonada.ferracaku@gmail.com",
                Id = "-1",
                LimpingTests = new List<LimpingTest>(),
                UserName = "jonada1",
            };
            _context.AppUsers.Add(defaultUser);
            await _context.SaveChangesAsync();
        }
    }
}
