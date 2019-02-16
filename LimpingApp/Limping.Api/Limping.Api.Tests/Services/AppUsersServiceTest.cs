using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Limping.Api.Tests.Services
{
    [Collection(nameof(WebHostCollection))]
    public class AppUsersServiceTest
    {
        private readonly WebHostFixture _fixture;

        public AppUsersServiceTest(WebHostFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task UserIsCreated()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>()
                    .Context;
                var user = new AppUser
                {
                    LimpingTests = new List<LimpingTest>(),
                    Id = "1",
                    UserName = "Jonada",
                    Email = "jonadaf@gmail.com"
                };
                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var userEntity = await service.Create(user);
                Assert.Equal(user, userEntity);
                var foundUser = context.AppUsers.Find("1");
                Assert.NotNull(foundUser);
            }
        }
    }
}
