﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Limping.Api.Extensions;
using Limping.Api.Models;
using Limping.Api.Services;
using Limping.Api.Services.Interfaces;
using Limping.Api.Services.Lifetimes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Limping.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connection = @"Server=(localdb)\mssqllocaldb;Database=LimpingDatabase;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<LimpingDbContext>
                (options => options.UseSqlServer(connection));

            services.AddTransient<ILimpingTestsService, LimpingTestsService>();
            services.AddTransient<IAppUsersService, AppUsersService>();
            services.AddTransient<ITestAnalysesService, TestAnalysesService>();

            services.Scan(source => ScanAssemblyOf<Startup>(source, services,
                new List<(Type LifetimeMarker, ServiceLifetime Lifetime)>
                {
                    (typeof(ITransientService), ServiceLifetime.Transient),
                    (typeof(IScopedService), ServiceLifetime.Scoped),
                    (typeof(ISingletonService), ServiceLifetime.Singleton)
                }));
        }

        // Dependency injection for classes
        protected static void ScanAssemblyOf<T>(ITypeSourceSelector source, IServiceCollection services, IEnumerable<(Type LifetimeMarker, ServiceLifetime Lifetime)> markers)
        {
            // https://github.com/khellang/Scrutor/blob/5516fe092594c5063f6ab885890b79b2bf91cc24/src/Scrutor/ReflectionExtensions.cs
            bool HasMatchingGenericArity(Type interfaceType, TypeInfo typeInfo)
            {
                if (typeInfo.IsGenericType)
                {
                    var interfaceTypeInfo = interfaceType.GetTypeInfo();

                    if (interfaceTypeInfo.IsGenericType)
                    {
                        var argumentCount = interfaceType.GenericTypeArguments.Length;
                        var parameterCount = typeInfo.GenericTypeParameters.Length;

                        return argumentCount == parameterCount;
                    }

                    return false;
                }

                return true;
            }

            Type GetRegistrationType(Type interfaceType, TypeInfo typeInfo)
            {
                if (typeInfo.IsGenericTypeDefinition)
                {
                    var interfaceTypeInfo = interfaceType.GetTypeInfo();

                    if (interfaceTypeInfo.IsGenericType)
                    {
                        return interfaceType.GetGenericTypeDefinition();
                    }
                }

                return interfaceType;
            }

            IEnumerable<Type> AliasSelector(Type type)
            {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.ImplementedInterfaces
                    .Where(@interface => HasMatchingGenericArity(@interface, typeInfo))
                    .Select(@interface => GetRegistrationType(@interface, typeInfo));
            }

            markers
                .Aggregate(source.FromAssemblyOf<T>(), (selector, tuple) => selector
                    .AddClasses(classes => classes.AssignableTo(tuple.LifetimeMarker))
                    .AsSelfWithAliases(services, tuple.Lifetime, AliasSelector)
                    .WithLifetime(tuple.Lifetime));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
