using System.Threading.Tasks;
using Limping.Api.Configurations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Limping.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            var webHost = CreateWebHostBuilder(args)
                .Build();
            var result = webHost.BuildDatabase().Result;
            result.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => services.Configure<LimpingConfiguration>(context.Configuration.GetSection("Limping")))
                .UseStartup<Startup>()
                .UseWebRoot("wwwroot");
        }

        public static async Task<IWebHost> BuildDatabase(this IWebHost webHost)
        {
            var services = webHost.Services;

            using (var scope = services.CreateScope())
            {
                var service = scope.ServiceProvider
                        .GetRequiredService<DatabaseStartup>();
                await service.Migrate().ApplyDataMigrations();
            }

            return webHost;
        }
    }
}
