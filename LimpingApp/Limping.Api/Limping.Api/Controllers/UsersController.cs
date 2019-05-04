﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Halcyon.HAL;
using Limping.Api.Constants;
using Limping.Api.Dtos;
using Limping.Api.Dtos.UserDtos.Produces;
using Limping.Api.Dtos.UserDtos.Responses;
using Limping.Api.Extensions;
using Limping.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : LimpingControllerBase
    {
        private readonly LimpingDbContext _context;
        private readonly IAppUsersService _appUsersService;
        public UsersController(LimpingDbContext context, IAppUsersService appUsersService)
        {
            _context = context;
            _appUsersService = appUsersService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllUsersProduces))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _appUsersService.GetAll();
            var response = new GetAllUsersResponse(users);
            return Ok(response);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserByIdProduces))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] string userId)
        {
            var exists = await _context.AppUsers.AnyAsync(usr => usr.Id == userId);
            if (!exists)
            {
                return NotFound();
            }
            var user = await _appUsersService.GetById(userId);
            var response = new GetUserResponse(user);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserByIdProduces))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                    new LinkExtended("self", $"{ControllerUrls.AppUsers}CreateUser", "Create user", LinkMethods.POST, nameof(CreateUserDto))
                );
                return Ok(response);
            }
        }

        [HttpPatch("{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EditUser([FromRoute]string userId, [FromBody] EditUserDto editUserDto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(editUserDto.UserName) && string.IsNullOrWhiteSpace(editUserDto.Email))
            {
                return BadRequest(ModelState);
            }
            var exists = await _context.AppUsers.AnyAsync(usr => usr.Id == userId);
            if (!exists)
            {
                return NotFound();
            }

            var foundUser = await _context.AppUsers.AsNoTracking().SingleAsync(user => user.Id == userId);
            if (!string.IsNullOrWhiteSpace(editUserDto.UserName))
            {
                var usernameExists = await _context.AppUsers.AnyAsync(usr => usr.UserName == editUserDto.UserName);
                if (usernameExists)
                {
                    return Conflict("Username exists");
                }

                foundUser.UserName = editUserDto.UserName;
            }

            if (!string.IsNullOrWhiteSpace(editUserDto.Email))
            {
                var emailExists = await _context.AppUsers.AnyAsync(usr => usr.Email == editUserDto.Email);
                if (emailExists)
                {
                    return Conflict("Email exists");
                }

                foundUser.Email = editUserDto.Email;
            }

            await _appUsersService.Edit(foundUser);
            var response = new GetUserResponse(
                foundUser,
                new LinkExtended("edit", $"{ControllerUrls.AppUsers}EditUser/{foundUser.Id}", "Edit user", LinkMethods.PATCH, nameof(EditUserDto))                
            );
            return Ok(response);
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseWithLinksOnly))]
        public async Task<IActionResult> DeleteUser([FromRoute] string userId)
        {
            var exists = await _context.AppUsers.AnyAsync(usr => usr.Id == userId);
            if (!exists)
            {
                return NotFound();
            }

            await _appUsersService.Delete(userId);
            var links = new List<Link>
            {
                new Link("self", $"/api/Users/DeleteUser/{userId}",null, "DELETE"),
                new Link("allUsers", "/api/Users/GetAllUsers",null, "GET")
            };
            return Ok(new HALResponse(null).AddLinks(links));
        }
    }
}