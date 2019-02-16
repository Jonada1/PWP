using System.Threading.Tasks;
using Limping.Api.Configurations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Limping.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => services.Configure<LimpingConfiguration>(config => context.Configuration.GetSection("Limping")))
                .UseStartup<Startup>()
                .UseWebRoot("wwwroot");
        }

        //public static async Task<IWebHost> BuildDatabase(this IWebHost webHost)
        //{
        //    var services = webHost.Services;

        //    using (var scope = services.CreateScope())
        //    {
        //        await scope.ServiceProvider
        //                .GetRequiredService<DatabaseStartup>()
        //                .EnsureConnection()
        //                .Migrate()
        //                .ApplyDataMigrations()
        //            ;
        //    }

        //    return webHost;
        //}
    }
}
