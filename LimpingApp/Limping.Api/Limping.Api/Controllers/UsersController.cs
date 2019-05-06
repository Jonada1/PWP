using System.Collections.Generic;
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
using Limping.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Controllers
{
    /// <summary>
    /// Manages all the actions for the users. Create, Delete, Edit, Get, Get All
    /// </summary>
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

        /// <summary>
        /// Fetches all users from the database. Transforms them into the UserDto
        /// Also adds links to the response
        /// </summary>
        /// <returns>The list of users as embedded and the links that can follow</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllUsersProduces))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _appUsersService.GetAll();
            // Transform it to a list of UserDto which are embedded and add links
            var response = new GetAllUsersResponse(users);
            return Ok(response);
        }

        /// <summary>
        /// Fetches a single user from the database
        /// Attaches the links to it and transforms it to a UserDto
        /// </summary>
        /// <param name="userId">The id of user which is being fetched</param>
        /// <returns>Returns not found if it doesn't exist, the user and the links otherwise</returns>
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
            // Transform it to a UserDto with Links
            var response = new GetUserResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Create a user and then return the dto
        /// </summary>
        /// <param name="userDto">The needed parameters to create a user</param>
        /// <returns>
        /// Bad request if the request is invalid.
        /// Conflict if a user with the same username or email exists.
        /// The user with the links otherwise
        /// </returns>
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
            // Check if a user with the same username or email exsists
            var hasConflict = _context
                .AppUsers
                .Any(user =>
                    user.UserName == userDto.UserName
                    || user.Email == userDto.Email
                );
            // Return conflict if it does
            if (hasConflict)
            {
                return Conflict();
            }
            else
            {
                // Create the user
                var user = new AppUser
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email
                };
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                // Transform the response into HAL
                var response = new GetUserResponse(
                    user,
                    LinkGenerator.Users.Create("self")
                );
                return Ok(response);
            }
        }

        /// <summary>
        /// Edits the user and returns it
        /// </summary>
        /// <param name="userId">The id of the user which will be edited</param>
        /// <param name="editUserDto">The dto for the patch request</param>
        /// <returns>
        /// If the user was not found not found
        /// If a user with same username or email exists returns conflict
        /// If the request invalid bad request
        /// 
        /// </returns>
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

            // Check if the user exists
            var exists = await _context.AppUsers.AnyAsync(usr => usr.Id == userId);
            if (!exists)
            {
                return NotFound();
            }
            
            // Check if the username conflicts with existing ones
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

            // Check if the email conflicts with existing ones
            if (!string.IsNullOrWhiteSpace(editUserDto.Email))
            {
                var emailExists = await _context.AppUsers.AnyAsync(usr => usr.Email == editUserDto.Email);
                if (emailExists)
                {
                    return Conflict("Email exists");
                }

                foundUser.Email = editUserDto.Email;
            }

            // Save the changes
            await _appUsersService.Edit(foundUser);
            // Form the HAL response
            var response = new GetUserResponse(
                foundUser,
                LinkGenerator.Users.Edit(foundUser.Id, "self")
            );
            return Ok(response);
        }


        /// <summary>
        /// Deletes the user and returns the hal response with links to navigate away
        /// </summary>
        /// <param name="userId">The id of the user which is being deleted</param>
        /// <returns>
        /// Not found if user not found
        /// A HAL response with links only otherwise
        /// </returns>
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
                LinkGenerator.Users.Delete(userId, "self"),
                LinkGenerator.Users.GetAll()
            };
            return Ok(new HALResponse(null).AddLinks(links));
        }
    }
}