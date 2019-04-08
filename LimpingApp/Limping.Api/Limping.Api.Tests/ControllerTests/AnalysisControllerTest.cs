using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Constants;
using Limping.Api.Dtos.TestAnalysisDtos;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Limping.Api.Tests.ControllerTests
{
    [Collection(nameof(WebHostCollection))]
    public class AnalysisControllerTest
    {
        private readonly WebHostFixture _fixture;
        private AppUser _defaultUser;
        private LimpingTest _defaultLimpingTest;
        public AnalysisControllerTest(WebHostFixture fixture)
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
        public async Task GetByIdNotFoundTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendGetByIdRequest(Guid.NewGuid()))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task EditNotFoundTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendEditTestRequest(Guid.NewGuid(), new ReplaceTestAnalysisDto
                {
                    Description = "Good result",
                    EndValue = 2,
                    LimpingSeverity = LimpingSeverityEnum.Low,
                }))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task GetByIdOkTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendGetByIdRequest(_defaultLimpingTest.TestAnalysis.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        private async Task<HttpResponseMessage> SendGetByIdRequest(Guid analysisId)
        {
            return await _fixture.Server.CreateClient()
                .GetAsync($"{ControllerUrls.Analysis}GetById/{analysisId}");
        }

        [Fact]
        public async Task EditBadRequestTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var badRequests = new List<Object>
                {
                    // Empty
                    new { },
                    // Missing end value
                    new
                    {
                        Description = "Hello",
                        LimpingSeverity = LimpingSeverityEnum.High,
                    },
                    new
                    {
                        EndValue = 1,
                    }
                };
                foreach (var limpingBadRequest in badRequests)
                {
                    using (var response = await SendEditTestRequest(_defaultLimpingTest.TestAnalysis.Id, limpingBadRequest))
                    {
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    }
                }
            }
        }

        [Fact]
        public async Task EditOkTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var goodRequests = new List<Object>
                {
                    new ReplaceTestAnalysisDto
                    {
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.High,
                    },
                    new ReplaceTestAnalysisDto
                    {
                        Description = "blabla",
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.Medium,
                    }
                };
                foreach (var goodRequest in goodRequests)
                {
                    using (var response = await SendEditTestRequest(_defaultLimpingTest.TestAnalysis.Id, goodRequest))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SendEditTestRequest(Guid analysisId, Object obj)
        {
            return await _fixture.Server.CreateClient()
                .PutAsync($"{ControllerUrls.Analysis}EditTestAnalysis/{analysisId}", CreateStringContent(obj));
        }

        private StringContent CreateStringContent(Object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                "application/json");
        }
    }
}
