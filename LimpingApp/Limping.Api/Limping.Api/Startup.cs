using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Halcyon.Web.HAL.Json;
using Limping.Api.Configurations;
using Limping.Api.Extensions;
using Limping.Api.Models;
using Limping.Api.Services;
using Limping.Api.Services.Interfaces;
using Limping.Api.Services.Lifetimes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scrutor;
using Swashbuckle.AspNetCore.Swagger;

namespace Limping.Api
{
    public class Startup
    {
        private readonly IOptions<LimpingConfiguration> _limpingConfigurations;
        public Startup(IConfiguration configuration, IOptions<LimpingConfiguration> limpingConfigurations)
        {
            Configuration = configuration;
            _limpingConfigurations = limpingConfigurations;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc();
            services.AddRouting(routeOptions => routeOptions.LowercaseUrls = true);
            services.AddDbContext<LimpingDbContext>
            (options => options.UseNpgsql
                (
            _limpingConfigurations.Value.Services?.Database.ConnectionString
                          ?? "Host=localhost;Port=5440;Database=LimpingDatabaseTest;Username=postgres;Password=postgres;"
                )
            );

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

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Limping Data of Patients API", Version = "v1" });
            });
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(o =>
            {
                o.PreSerializeFilters.Add((document, request) =>
                 {
                     document.Paths = document.Paths.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value);
                 });
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Limping Data of Patients API V1");
            });

            app.UseMvc();
        }
    }
}
