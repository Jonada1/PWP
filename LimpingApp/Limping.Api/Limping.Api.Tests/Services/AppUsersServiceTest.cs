using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services.Interfaces;
using Limping.Api.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
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
        public async Task UserCreation()
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

        [Fact]
        public async Task UserCreationFails()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>()
                    .Context;
                var user1 = new AppUser
                {
                    LimpingTests = new List<LimpingTest>(),
                    Id = "1",
                    UserName = "Jonada",
                    Email = "jonadaf@gmail.com"
                };
                context.AppUsers.Add(user1);
                await context.SaveChangesAsync();
                context.Entry(user1).State = EntityState.Detached;


                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                await Assert.ThrowsAnyAsync<Exception>(() => service.Create(user1));
                var user2 = new AppUser
                {
                    LimpingTests = new List<LimpingTest>(),
                    Id = "2",
                    UserName = "Jonada",
                    Email = "jonadaf1@gmail.com"
                };
                await Assert.ThrowsAnyAsync<Exception>(() => service.Create(user2));
                var user3 = new AppUser
                {
                    LimpingTests = new List<LimpingTest>(),
                    Id = "3",
                    UserName = "Jonada1",
                    Email = "jonadaf@gmail.com"
                };
                await Assert.ThrowsAnyAsync<Exception>(() => service.Create(user2));

            }
        }

        [Fact]
        public async Task UserEdit()
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
                context.AppUsers.Add(user);
                await context.SaveChangesAsync();
                context.Entry(user).State = EntityState.Detached;

                var edited = new AppUser
                {
                    LimpingTests = new List<LimpingTest>(),
                    Id = "1",
                    UserName = "Jnd",
                    Email = "das@gmail.com"
                };

                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var userEntity = await service.Edit(edited);
                Assert.Equal(edited, userEntity);
                var foundUser = context.AppUsers.Find("1");
                Assert.Equal(edited, foundUser);
                Assert.NotNull(foundUser);

            }
        }

        [Fact]
        public async Task UserDelete()
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
                context.AppUsers.Add(user);
                await context.SaveChangesAsync();
                context.Entry(user).State = EntityState.Detached;
                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                await service.Delete("1");
                var doesUserExist = await context.AppUsers.AnyAsync(x => x.Id == "1");
                Assert.False(doesUserExist);
            }
        }

        [Fact]
        public async Task UserDeleteNonExistentThrows()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                await Assert.ThrowsAnyAsync<Exception>(() => service.Delete("1"));
            }
        }

        [Fact]
        public async Task UserGetByIdWorks()
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
                context.AppUsers.Add(user);
                await context.SaveChangesAsync();
                context.Entry(user).State = EntityState.Detached;
                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var getUser = await service.GetById("1");
                Assert.Equal(user.Id,getUser.Id);
                var nullUser = await service.GetById("2");
                Assert.Null(nullUser);
            }
        }
        [Fact]
        public async Task UserGetAllUsersSuccess()
        {
            using (var scope = _fixture.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseFixture>()
                    .Context;
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        LimpingTests = new List<LimpingTest>(),
                        Id = "1",
                        UserName = "Jonada1",
                        Email = "jonadaf1@gmail.com"
                    },
                    new AppUser
                    {
                        LimpingTests = new List<LimpingTest>(),
                        Id = "2",
                        UserName = "Jonada2",
                        Email = "jonadaf2@gmail.com"
                    }
                };
                context.AppUsers.AddRange(users);
                await context.SaveChangesAsync();
                var service = scope.ServiceProvider.GetRequiredService<IAppUsersService>();
                var getUsers = await service.GetAll();
                Assert.Equal(2, getUsers.Count);
                Assert.Equal(users, getUsers);
            }
        }
    }
}
