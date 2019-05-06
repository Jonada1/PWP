using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Constants;
using Limping.Api.Dtos.LimpingTestDtos;
using Limping.Api.Dtos.TestAnalysisDtos;
using Limping.Api.Dtos.UserDtos;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Limping.Api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Limping.Api.Tests.ControllerTests
{
    [Collection(nameof(WebHostCollection))]
    public class LimpingTestsControllerTest
    {
        private readonly WebHostFixture _fixture;
        private AppUser _defaultUser;
        private LimpingTest _defaultLimpingTest;
        public LimpingTestsControllerTest(WebHostFixture fixture)
        {
            _fixture = fixture;
        }

        #region Scopes for the request

        // Create scopes that live in a single test 

        private async Task<IServiceScope> CreateScopeWithUserAsync()
        {
            var scope = _fixture.Server.Host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
            var appUsersService = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
            _defaultUser = await appUsersService.Create(new Models.AppUser
            {
                Email = "jonada1@gmail.com",
                UserName = "jonada1"
            });
            return scope;
        }
        private async Task<IServiceScope> CreateScopeWithLimpingTestAsync()
        {
            var scope = await CreateScopeWithUserAsync();
            var limpingTestsService = scope.ServiceProvider.GetRequiredService<ILimpingTestsService>();
            _defaultLimpingTest = await limpingTestsService.InsertTest(_defaultUser.Id, "{'a': 'b'}", new TestAnalysis
            {
                Description = "Good result",
                EndValue = 2,
                LimpingSeverity = LimpingSeverityEnum.Low,
            });
            return scope;
        }
        #endregion


        [Fact]
        public async Task GetAll()
        {
            // Test that get all returns Ok
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendGetAll())
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendGetAll()
        {
            return await _fixture.Server
                .CreateRequest(LinkGenerator.LimpingTests.GetAll().Href).GetAsync();
        }

        [Fact]
        public async Task GetForUserTest()
        {
            using (var scope = await CreateScopeWithUserAsync())
            {
                // Test Ok response
                using (var response = await SendGetForUserRequest(_defaultUser.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // Test User not found response
                using (var response = await SendGetForUserRequest(_defaultUser.Id + "1"))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendGetForUserRequest(string userId)
        {
            return await _fixture.Server
                .CreateRequest(LinkGenerator.LimpingTests.GetForUser(userId).Href).GetAsync();
        }

        [Fact]
        public async Task GetByIdTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                // Test ok
                using (var response = await SendGetByIdRequest(_defaultLimpingTest.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                // Test not found
                using (var response = await SendGetByIdRequest(Guid.NewGuid()))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendGetByIdRequest(Guid limpingTestId)
        {
            return await _fixture.Server
                .CreateRequest(LinkGenerator.LimpingTests.GetSingle(limpingTestId.ToString()).Href).GetAsync();
        }

        [Fact]
        public async Task EditNotFoundTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                // Test not found for non existent test
                using (var response = await SendEditRequest(Guid.NewGuid(), new EditLimpingTestDto
                {
                    TestData = "'a':'b'",
                }))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task EditBadRequestTest()
        {
            // Test bad request scenarios for edit
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var badRequestObjects = new List<Object>
                {
                    // Empty
                    new { },
                    // Test data empty
                    new EditLimpingTestDto
                    {
                        TestData = ""
                    },
                    // Test data null
                    new EditLimpingTestDto
                    {
                        TestData = null
                    },
                    //  Missing test data with provided test analysis
                    new EditLimpingTestDto
                    {
                        TestAnalysis = new ReplaceTestAnalysisDto
                        {
                            Description = "bla",
                            EndValue = 1,
                            LimpingSeverity = LimpingSeverityEnum.Low,
                        }
                    }
                };
                foreach (var limpingBadRequestObject in badRequestObjects)
                {
                    using (var response = await SendEditRequest(_defaultLimpingTest.Id, limpingBadRequestObject))
                    {
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    }
                }
            }
        }

        [Fact]
        public async Task EditOkRequestTest()
        {
            // Test Ok requests for edit
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var goodRequests = new List<Object>
                {
                    // Proper test data, missing analysis
                    new EditLimpingTestDto
                    {
                        TestData = "'a': 'b'"
                    },
                    // Proper test data and test analysis
                    new EditLimpingTestDto
                    {
                        TestData = "'a': 'b'",
                        TestAnalysis = new ReplaceTestAnalysisDto
                        {
                            Description = "bla",
                            EndValue = 1,
                            LimpingSeverity = LimpingSeverityEnum.Low,
                        }
                    }
                };
                foreach (var goodRequest in goodRequests)
                {
                    using (var response = await SendEditRequest(_defaultLimpingTest.Id, goodRequest))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    }
                }
            }

        }

        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendEditRequest(Guid limpingTestId, Object obj)
        {
            return await _fixture.Server.CreateClient()
                .PatchAsync(LinkGenerator.LimpingTests.Edit(limpingTestId.ToString()).Href, CreateStringContent(obj));
        }

        [Fact]
        public async Task CreateOkRequestTest()
        {
            // Test successful create
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendCreateRequest(new CreateLimpingTestDto
                {
                    TestData = "'a':'b'",
                    AppUserId = _defaultUser.Id,
                    TestAnalysis = new ReplaceTestAnalysisDto
                    {
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.High,
                        Description = "blabla"
                    }
                }))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task CreateNotFoundRequestTest()
        {
            // Test case when user doesn't exist scenario. Should return not found
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendCreateRequest(new CreateLimpingTestDto
                {
                    TestData = "'a':'b'",
                    AppUserId = Guid.NewGuid().ToString(),
                    TestAnalysis = new ReplaceTestAnalysisDto
                    {
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.High,
                        Description = "blabla"
                    }
                }))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task CreateBadRequestTest()
        {
            // Test bad request scenarios for create
            using (var scope = await CreateScopeWithUserAsync())
            {
                var badRequests = new List<Object>
                {
                    // Empty
                    new { },
                    // Missing test data
                    new CreateLimpingTestDto
                    {
                        AppUserId = _defaultUser.Id,
                        TestAnalysis = new ReplaceTestAnalysisDto
                        {
                            EndValue = 1,
                            LimpingSeverity = LimpingSeverityEnum.High,
                            Description = "blabla"
                        }
                    },
                    // Missing user id
                    new CreateLimpingTestDto
                    {
                        TestData = "'a':'b'",
                        TestAnalysis = new ReplaceTestAnalysisDto
                        {
                            EndValue = 1,
                            LimpingSeverity = LimpingSeverityEnum.High,
                            Description = "blabla"
                        }
                    },
                    // Missing test analysis
                    new CreateLimpingTestDto
                    {
                        TestData = "'a':'b'",
                        AppUserId = _defaultUser.Id,
                    },
                    // Bad test analysis
                    new CreateLimpingTestDto 
                    {
                        TestData = "'a': 'b'",
                        AppUserId = _defaultUser.Id,
                        TestAnalysis = new ReplaceTestAnalysisDto
                        {
                            Description = "blabla",
                        }
                    }

                };
                foreach (var limpingBadRequest in badRequests)
                {
                    using (var response = await SendCreateRequest(limpingBadRequest))
                    {
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    }
                }
            }
        }
        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendCreateRequest(Object obj)
        {
            return await _fixture.Server.CreateClient()
                .PostAsync(LinkGenerator.LimpingTests.Create().Href, CreateStringContent(obj));
        }

        [Fact]
        public async Task DeleteOkTest()
        {
            // Test ok scenario for deletion of limping test
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendDeleteRequest(_defaultLimpingTest.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>().Context;
                Assert.False(await context.LimpingTests.AnyAsync(x => x.Id == _defaultLimpingTest.Id));
            }
        }

        [Fact]
        public async Task DeleteNotFoundTest()
        {
            // Test not found for test deletion that doesn't exist
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendDeleteRequest(Guid.NewGuid()))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        /// <summary>
        /// Send the request to virtual server
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendDeleteRequest(Guid limpingId)
        {
            return await _fixture.Server.CreateClient()
                .DeleteAsync(LinkGenerator.LimpingTests.Delete(limpingId.ToString()).Href);
        }


        /// <summary>
        /// Create content body string to send with the requests
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private StringContent CreateStringContent(Object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                "application/json");
        }
    }
}
