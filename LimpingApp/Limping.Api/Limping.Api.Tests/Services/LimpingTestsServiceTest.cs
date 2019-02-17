﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.DataMigrations;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Limping.Api.Tests.Services
{
    [Collection(nameof(WebHostCollection))]
    public class LimpingTestsServiceTest
    {
        private readonly WebHostFixture _fixture;

        public LimpingTestsServiceTest(WebHostFixture fixture)
        {
            _fixture = fixture;
        }

        private TestAnalysis GetTestAnalysis()
        {
            return new TestAnalysis
            {
                Description = "Something",
                EndValue = 1.5,
                LimpingSeverity = LimpingSeverityEnum.Medium,
            };
        }

        private async Task AddDefaultDataForLimping(LimpingDbContext context)
        {
            context.AppUsers.Add(new AppUser
            {
                Id = "1",
                UserName = "f",
                LimpingTests = new List<LimpingTest>(),
                Email = "f",
            });
            await context.SaveChangesAsync();
        }

        private static readonly string _data = "{numbers: [1, 2, 3]}";

        [Fact]
        public async Task LimpingTestInsert()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var databaseFixture = scope.ServiceProvider.GetRequiredService<DatabaseFixture>();
                var context = databaseFixture.Context;
                await AddDefaultDataForLimping(context);

                var service = scope.ServiceProvider.GetRequiredService<ILimpingTestsService>();
                var added = await service.InsertTest("1", _data, GetTestAnalysis());
                context.Entry(added).State = EntityState.Detached;
                var foundTest = await context.LimpingTests.FindAsync(added.Id);
                Assert.NotNull(foundTest);
                await Assert.ThrowsAnyAsync<Exception>(() => service.InsertTest("2", _data, GetTestAnalysis()));
            }
        }

        [Fact]
        public async Task LimpingTestDelete()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var databaseFixture = scope.ServiceProvider.GetRequiredService<DatabaseFixture>();
                var context = databaseFixture.Context;
                await AddDefaultDataForLimping(context);
                var service = scope.ServiceProvider.GetRequiredService<ILimpingTestsService>();
                var limpingTest = new LimpingTest
                {
                    AppUserId = "1",
                    Date = DateTime.Now,
                    TestAnalysis = new TestAnalysis(),
                    TestData = _data,
                };
                context.LimpingTests.Add(limpingTest);
                await context.SaveChangesAsync();
                await service.DeleteTest(limpingTest.Id);
                Assert.Equal(_data, limpingTest.TestData);
                var nullTest = await context.LimpingTests.FindAsync(limpingTest.Id);
                Assert.Null(nullTest);
                await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteTest(limpingTest.Id));
            }
        }

        [Fact]
        public async Task LimpingTestEdit()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var databaseFixture = scope.ServiceProvider.GetRequiredService<DatabaseFixture>();
                var context = databaseFixture.Context;
                await AddDefaultDataForLimping(context);
                var service = scope.ServiceProvider.GetRequiredService<ILimpingTestsService>();
                var limpingTest = new LimpingTest
                {
                    AppUserId = "1",
                    Date = DateTime.Now,
                    TestAnalysis = new TestAnalysis(),
                    TestData = _data,
                };
                context.LimpingTests.Add(limpingTest);
                await context.SaveChangesAsync();
                var newTestData = "{numbers: [1,2]}";
                var edited = await service.EditTest(limpingTest.Id, newTestData, GetTestAnalysis());
                Assert.Equal(newTestData, edited.TestData);
                await Assert.ThrowsAnyAsync<Exception>(() =>
                    service.EditTest(Guid.NewGuid(), "{numbers: [1, 2, 3]}", GetTestAnalysis()));
            }
        }

        [Fact]
        public async Task LimpingTestGet()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var databaseFixture = scope.ServiceProvider.GetRequiredService<DatabaseFixture>();
                var context = databaseFixture.Context;
                await AddDefaultDataForLimping(context);
                var service = scope.ServiceProvider.GetRequiredService<ILimpingTestsService>();
                var limpingTest = new LimpingTest
                {
                    AppUserId = "1",
                    Date = DateTime.Now,
                    TestAnalysis = new TestAnalysis(),
                    TestData = _data,
                };
                context.LimpingTests.Add(limpingTest);
                await context.SaveChangesAsync();
                var foundTest = await service.GetById(limpingTest.Id);
                Assert.Equal(limpingTest.Id, foundTest.Id);
                var nullTest = await service.GetById(Guid.NewGuid());
                Assert.Null(nullTest);
                var testsOfUser = await service.GetUserTests("1");
                Assert.Single(testsOfUser);
            }
        }
    }
}
