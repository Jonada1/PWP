using System.Collections.Generic;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Services
{
    public class AppUsersService: IAppUsersService
    {
        private readonly LimpingDbContext _context;

        public AppUsersService(LimpingDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetById(string id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            return user;
        }

        public async Task<List<AppUser>> GetAll()
        {
            return await _context.AppUsers.ToListAsync();
        }

        public async Task<AppUser> Create(AppUser user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(string id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> Edit(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
