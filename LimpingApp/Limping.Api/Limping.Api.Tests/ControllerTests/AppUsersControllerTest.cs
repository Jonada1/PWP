using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Constants;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Models;
using Limping.Api.Services;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Limping.Api.Tests.ControllerTests
{
    [Collection(nameof(WebHostCollection))]
    public class AppUsersControllerTest
    {
        private readonly WebHostFixture _fixture;

        public AppUsersControllerTest(WebHostFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAll()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                using (var response = await SendGetAll())
                {

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        private async Task<HttpResponseMessage> SendGetAll()
        {
            return await _fixture.Server
                .CreateRequest($"{ControllerUrls.AppUsers}GetAllUsers").GetAsync();
        }
        [Fact]
        public async Task GetUserByIdTest()
        {
            // Creates the scope for the server which grabs the services
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                // Forces to create db
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                // Test not found
                using (var response = await SendGetUserById("1"))
                {

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }

                // Test ok
                var appUserService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var createdUser = await appUserService.Create(new AppUser
                {
                    Email = "n@n.gmail.com",
                    UserName = "n94",
                });
                using (var response = await SendGetUserById(createdUser.Id))
                {

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        private async Task<HttpResponseMessage> SendGetUserById(string userId)
        {
            return await _fixture.Server
                .CreateRequest($"{ControllerUrls.AppUsers}GetById/{userId}").GetAsync();
        }
        [Fact]
        public async Task CreateUserBadRequestTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                // Test bad request with empty body
                using (var response = await SendCreateUserRequest(new { }))
                {

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }

                // Test bad request with username missing
                var missingUsername = new
                {
                    Email = "n@gmail.com"
                };
                using (var response = await SendCreateUserRequest(missingUsername))
                {

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                // Test bad request with username missing
                var missingEmail = new
                {
                    UserName = "n@gmail.com"
                };
                using (var response = await SendCreateUserRequest(missingEmail))
                {

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task CreateUserOkTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;

                // Test Ok request
                var goodRequest = new CreateUserDto
                {

                    Email = "jonada1@gmail.com",
                    UserName = "jonada1"
                };
                using (var response = await SendCreateUserRequest(goodRequest))
                {

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task CreateUserConflictTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                var appUserService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var createdUser = await appUserService.Create(new AppUser
                {
                    Email = "jonada1@gmail.com",
                    UserName = "jonada1",
                });
                // Test Email conflict request
                var emailConflict = new CreateUserDto
                {

                    Email = createdUser.Email,
                    UserName = "jonada2"
                };
                using (var response = await SendCreateUserRequest(emailConflict))
                {

                    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
                }

                // Test Email conflict request
                var usernameConflict = new CreateUserDto
                {

                    Email = "randomEmail@email.com",
                    UserName = createdUser.UserName
                };

                using (var response = await SendCreateUserRequest(usernameConflict))
                {

                    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
                }
            }
        }

        private async Task<HttpResponseMessage> SendCreateUserRequest(Object body)
        {
            return await _fixture.Server
                .CreateClient().PostAsync($"{ControllerUrls.AppUsers}CreateUser", CreateStringContent(body));
        }
        [Fact]
        public async Task EditOkTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                var appUserService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var createdUser = await appUserService.Create(new AppUser
                {
                    Email = "jonada1@gmail.com",
                    UserName = "jonada1"
                });

                // Edit email
                var editEmail = new EditUserDto
                {
                    Email = "jonada2@gmail.com",
                };
                using (var response = await SendEditUserRequest(createdUser.Id, editEmail))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // Edit username
                var editUsername = new EditUserDto
                {
                    UserName = "jonada2",
                };
                using (var response = await SendEditUserRequest(createdUser.Id, editUsername))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // edit both
                var editBoth = new EditUserDto
                {
                    UserName = "jonada3",
                    Email = "jonada3@gmail.com",
                };
                using (var response = await SendEditUserRequest(createdUser.Id, editBoth))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
        [Fact]
        public async Task EditBadRequestAndNotFoundTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                // Test bad request
                using (var response = await SendEditUserRequest("1", new { }))
                {
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                // Test not found
                var editRequest = new EditUserDto
                {
                    Email = "jonada1@gmail.com",
                };
                using (var response = await SendEditUserRequest("1", editRequest))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }
        [Fact]
        public async Task EditConflictTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                var appUserService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var createdUser = await appUserService.Create(new AppUser
                {
                    Email = "jonada1@gmail.com",
                    UserName = "jonada1"
                });

                // Test email conflict
                var editRequest = new EditUserDto
                {
                    Email = createdUser.Email,
                };
                using (var response = await SendEditUserRequest(createdUser.Id, editRequest))
                {
                    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
                }
                // Test username conflict
                editRequest = new EditUserDto
                {
                    UserName = createdUser.UserName
                };

                using (var response = await SendEditUserRequest(createdUser.Id, editRequest))
                {
                    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
                }

            }

        }
        private async Task<HttpResponseMessage> SendEditUserRequest(string userId, Object body)
        {
            return await _fixture.Server.CreateClient()
                .PatchAsync($"{ControllerUrls.AppUsers}EditUser/{userId}", CreateStringContent(body));
        }

        [Fact]
        public async Task DeleteNotFoundTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                using (var response = await SendDeleteUserRequest("123"))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task DeleteOkTest()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                var appUsersService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var createdUser = await appUsersService.Create(new AppUser
                {
                    UserName = "jonada1",
                    Email = "jonada1@gmail.com",
                });

                // Response was correct
                using (var response = await SendDeleteUserRequest(createdUser.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // Assert that it was deleted
                Assert.False(context.AppUsers.Any(x => x.Id == createdUser.Id));
            }
        }

        private async Task<HttpResponseMessage> SendDeleteUserRequest(string userId)
        {
            return await _fixture.Server.CreateClient()
                .DeleteAsync($"{ControllerUrls.AppUsers}DeleteUser/{userId}");
        }
        private StringContent CreateStringContent(Object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                "application/json");
        }
    }
}
