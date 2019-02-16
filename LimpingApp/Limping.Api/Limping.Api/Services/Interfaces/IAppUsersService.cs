using System.Collections.Generic;
using System.Threading.Tasks;
using Limping.Api.Models;

namespace Limping.Api.Services.Interfaces
{
    public interface IAppUsersService
    {
        Task<AppUser> GetById(string id);
        Task<List<AppUser>> GetAll();
        Task<AppUser> Create(AppUser user);
        Task Delete(string id);
        Task<AppUser> Edit(AppUser user);
    }
}
