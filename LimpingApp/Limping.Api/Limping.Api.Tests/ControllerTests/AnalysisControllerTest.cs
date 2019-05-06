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
using Limping.Api.Utils;
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
            // The fixture is a virtual server that lives for a single test method
            _fixture = fixture;
        }

        #region Scope creation for requests

        // Create the scopes in this section. Scopes live the same as the lifetime of a single test method

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
        public async Task GetByIdNotFoundTest()
        {
            // Test that a random guid should return not found
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendGetByIdRequest(Guid.NewGuid()))
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task GetByIdOkTest()
        {
            // Test that a get is successful when an existing test analysis is provided
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                using (var response = await SendGetByIdRequest(_defaultLimpingTest.TestAnalysis.Id))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }

        /// <summary>
        /// For the get by id request that will be sent by the virtual server
        /// </summary>
        /// <param name="analysisId">The analysis id</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendGetByIdRequest(Guid analysisId)
        {
            return await _fixture.Server.CreateClient()
                .GetAsync(LinkGenerator.Analysis.GetSingle(analysisId.ToString()).Href);
        }

        [Fact]
        public async Task EditNotFoundTest()
        {
            // Test that a user edit should return not found with a random guid
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
        public async Task EditBadRequestTest()
        {
            // Test that bad requests fail
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
                    // Missing severity
                    new
                    {
                        EndValue = 1,
                    }
                };
                
                // Test each of the bodies above
                foreach (var limpingBadRequest in badRequests)
                {
                    using (var response = await SendEditTestRequest(_defaultLimpingTest.Id, limpingBadRequest))
                    {
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    }
                }
            }
        }

        [Fact]
        public async Task EditOkTest()
        {
            // Test that a good edit request goes through with ok
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var goodRequests = new List<Object>
                {
                    // Good request without description
                    new ReplaceTestAnalysisDto
                    {
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.High,
                    },
                    // Good request with description
                    new ReplaceTestAnalysisDto
                    {
                        Description = "blabla",
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.Medium,
                    }
                };

                // Test each of the bodies above
                foreach (var goodRequest in goodRequests)
                {
                    using (var response = await SendEditTestRequest(_defaultLimpingTest.Id, goodRequest))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the edit test request from the virtual server
        /// </summary>
        /// <param name="analysisId">The analysis</param>
        /// <param name="obj">The body of the request</param>
        /// <returns>The request response</returns>
        private async Task<HttpResponseMessage> SendEditTestRequest(Guid analysisId, Object obj)
        {
            var url = LinkGenerator.Analysis.Edit(analysisId.ToString()).Href;
            return await _fixture.Server.CreateClient()
                .PutAsync(url, CreateStringContent(obj));
        }

        [Fact]
        public async Task ReplaceOkTest()
        {
            using (var scope = await CreateScopeWithLimpingTestAsync())
            {
                var goodRequests = new List<Object>
                {
                    // Good request without description
                    new ReplaceTestAnalysisDto
                    {
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.High,
                    },
                    // Good request with description
                    new ReplaceTestAnalysisDto
                    {
                        Description = "blabla",
                        EndValue = 1,
                        LimpingSeverity = LimpingSeverityEnum.Medium,
                    }
                };

                // Test each of the bodies above
                foreach (var goodRequest in goodRequests)
                {
                    using (var response = await SendReplacteAnalysisRequest(_defaultLimpingTest.TestAnalysis.Id, goodRequest))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    }
                }
            }
        }

        [Fact]
        public async Task ReplaceBadRequestTest()
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
                    // Missing severity
                    new
                    {
                        EndValue = 1,
                    }
                };

                // Test each of the bodies above
                foreach (var badRequest in badRequests)
                {
                    using (var response = await SendReplacteAnalysisRequest(_defaultLimpingTest.TestAnalysis.Id, badRequest))
                    {
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the replace test request from the virtual server
        /// </summary>
        /// <param name="analysisId">The analysis</param>
        /// <param name="obj">The body of the request</param>
        /// <returns>The request response</returns>
        private async Task<HttpResponseMessage> SendReplacteAnalysisRequest(Guid analysisId, Object obj)
        {
            var url = LinkGenerator.Analysis.Replace(analysisId.ToString()).Href;
            return await _fixture.Server.CreateClient()
                .PutAsync(url, CreateStringContent(obj));
        }

        /// <summary>
        /// Create the body as a string content with json type
        /// </summary>
        /// <param name="obj">The body in C#</param>
        /// <returns>The json as a content string</returns>
        private StringContent CreateStringContent(Object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                "application/json");
        }
    }
}
