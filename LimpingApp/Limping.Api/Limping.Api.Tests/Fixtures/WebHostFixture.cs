using System;
using System.IO;
using System.Threading.Tasks;
using Limping.Api.Configurations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Limping.Api.Tests.Fixtures
{
    public class WebHostFixture : IDisposable
    {
        private readonly string _webRoot;

        public WebHostFixture()
        {
            _webRoot = Path.Combine(Path.GetTempPath(), "testing");
            try
            {
                Directory.Delete(_webRoot, true);
            }
            catch (DirectoryNotFoundException)
            {
                //Do nothing
            }

            Directory.CreateDirectory(_webRoot);
            var webHostBuilder = WebHost
                .CreateDefaultBuilder();
            webHostBuilder
                .UseEnvironment("Testing")
                .ConfigureServices(services => services.AddOptions())
                .ConfigureServices(ConfigureServices)
                .UseStartup<Startup>()
                .UseWebRoot(_webRoot)
                .ConfigureServices(services => services.AddScoped<DatabaseFixture>());
            Server = new TestServer(webHostBuilder);
        }

        void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            var config = context.Configuration.GetSection("LimpingTests");
            services.Configure<LimpingConfiguration>(config);
        }

        public TestServer Server { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Server.Dispose();
                Directory.Delete(_webRoot, true);
            }
        }
    }
}
