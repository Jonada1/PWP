using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Limping.Api.Tests.Services
{
    [Collection(nameof(WebHostCollection))]
    public class TestAnalysesServiceTest
    {
        private readonly WebHostFixture _fixture;

        public TestAnalysesServiceTest(WebHostFixture fixture)
        {
            _fixture = fixture;
        }

        private async Task<LimpingTest> AddDefaultDataForAnalysis(LimpingDbContext context)
        {
            var analysis = new TestAnalysis
            {
                Id = Guid.NewGuid(),
                Description = "Something",
                EndValue = 1.5,
                LimpingSeverity = LimpingSeverityEnum.Medium,
            };
            var appUser = context.AppUsers.Add(new AppUser
            {
                Id = "1",
                UserName = "f",
                LimpingTests = new List<LimpingTest>(),
                Email = "f",
            }).Entity;
            var limpingTest = context.LimpingTests.Add(new LimpingTest
            {
                AppUserId = appUser.Id,
                Date = DateTime.Now,
                TestAnalysis = analysis,
                TestData = "{numbers: [1, 2, 3]}"
            }).Entity;
            await context.SaveChangesAsync();
            context.Entry(limpingTest).State = EntityState.Detached;
            return limpingTest;
        }

        [Fact]
        public async Task TestAnalysesTesting()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var databaseFixture = scope.ServiceProvider.GetRequiredService<DatabaseFixture>();
                var context = databaseFixture.Context;
                var limpingTest = await AddDefaultDataForAnalysis(context);
                var service = scope.ServiceProvider.GetRequiredService<ITestAnalysesService>();
                var editAnalysis = new TestAnalysis
                {
                    Id = limpingTest.TestAnalysis.Id,
                    Description = "ok",
                    EndValue = 1.9,
                    LimpingSeverity = LimpingSeverityEnum.High,
                };
                await service.EditTest(editAnalysis);
                var found = await context.TestAnalyses.FindAsync(editAnalysis.Id);
                Assert.Equal(editAnalysis.Description, found.Description);
                Assert.Equal(editAnalysis.EndValue, found.EndValue);
                Assert.Equal(editAnalysis.LimpingSeverity, found.LimpingSeverity);
                limpingTest.TestAnalysis.Id = Guid.NewGuid();
                await Assert.ThrowsAnyAsync<Exception>(() => service.EditTest(limpingTest.TestAnalysis));
                var newAnalysis = new TestAnalysis
                {
                    Id = Guid.NewGuid(),
                    Description = "kk",
                    EndValue = 1.5,
                    LimpingSeverity = LimpingSeverityEnum.High
                };
                await service.ReplaceTest(editAnalysis.Id, newAnalysis);

                var replacedAnalysis = await context.TestAnalyses.FindAsync(newAnalysis.Id);
                var nullAnalysis = await context.TestAnalyses.FindAsync(editAnalysis.Id);
                Assert.NotNull(replacedAnalysis);
                Assert.Null(nullAnalysis);
            }
        }
    }
}
