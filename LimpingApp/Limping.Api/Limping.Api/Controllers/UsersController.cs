using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Models;
using Limping.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Halcyon.HAL;
using Halcyon.Web.HAL;
using Limping.Api.Services.Interfaces;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LimpingDbContext _context;
        private readonly IAppUsersService _appUsersService;
        public UsersController(LimpingDbContext context, IAppUsersService appUsersService)
        {
            _context = context;
            _appUsersService = appUsersService;
        }

        [HttpGet("[action]/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserByIdProduces))]
        public async Task<IActionResult> GetById([FromRoute] string userId)
        {
            var exists = _context.AppUsers.Any(usr => usr.Id == userId);
            if(!exists)
            {
                return NotFound();
            }
            var user = await _appUsersService.GetById(userId);
            var response = new GetUserResponse(
                user, 
                new List<Link> {
                    new Link("self", $"/api/Users/GetById/{userId}")
                }
            );
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hasConflict = _context
                .AppUsers
                .Any(user =>
                    user.UserName == userDto.UserName
                    || user.Email == userDto.Email
                );
            if (hasConflict)
            {
                return Conflict();
            }
            else
            {
                var user = new AppUser
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email
                };
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                var response = new GetUserResponse(
                user,
                new List<Link> {
                    new Link("self", "/api/Users/CreateUser"),
                    new Link("get", $"/api/Users/GetById/{user.Id}")
                }
            );
                return Ok(response);
            }
        }
    }
}