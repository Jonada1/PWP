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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SQLitePCL;
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

        [Fact]
        public async Task GetAll()
        {
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendGetAll())
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
        private async Task<HttpResponseMessage> SendGetAll()
        {
            return await _fixture.Server
                .CreateRequest($"{ControllerUrls.LimpingTests}GetAll").GetAsync();
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

        private async Task<HttpResponseMessage> SendGetForUserRequest(string userId)
        {
            return await _fixture.Server
                .CreateRequest($"{ControllerUrls.LimpingTests}GetForUser/{userId}").GetAsync();
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

        private async Task<HttpResponseMessage> SendGetByIdRequest(Guid limpingTestId)
        {
            return await _fixture.Server
                .CreateRequest($"{ControllerUrls.LimpingTests}GetById/{limpingTestId}").GetAsync();
        }

        [Fact]
        public async Task EditNotFoundTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                // Test not found
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
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var badRequestObjects = new List<Object>
                {
                    new { },
                    new EditLimpingTestDto
                    {
                        TestData = ""
                    },
                    new EditLimpingTestDto
                    {
                        TestData = null
                    },
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
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var goodRequests = new List<Object>
                {
                    new EditLimpingTestDto
                    {
                        TestData = "'a': 'b'"
                    },
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
        private async Task<HttpResponseMessage> SendEditRequest(Guid limpingTestId, Object obj)
        {
            return await _fixture.Server.CreateClient()
                .PatchAsync($"{ControllerUrls.LimpingTests}Edit/{limpingTestId}", CreateStringContent(obj));
        }

        [Fact]
        public async Task CreateOkRequestTest()
        {
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
            using (var scope = await CreateScopeWithUserAsync())
            {
                var badRequests = new List<Object>
                {
                    new { },
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
                    new CreateLimpingTestDto
                    {
                        TestData = "'a':'b'",
                        AppUserId = _defaultUser.Id,
                    },
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
        private async Task<HttpResponseMessage> SendCreateRequest(Object obj)
        {
            return await _fixture.Server.CreateClient()
                .PostAsync($"{ControllerUrls.LimpingTests}Create", CreateStringContent(obj));
        }

        [Fact]
        public async Task DeleteOkTest()
        {
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
            using (var scope = await CreateScopeWithUserAsync())
            {
                using (var response = await SendDeleteRequest(Guid.NewGuid()))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }
        private async Task<HttpResponseMessage> SendDeleteRequest(Guid limpingId)
        {
            return await _fixture.Server.CreateClient()
                .DeleteAsync($"{ControllerUrls.LimpingTests}Delete/{limpingId}");
        }

        private StringContent CreateStringContent(Object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                "application/json");
        }
    }
}
