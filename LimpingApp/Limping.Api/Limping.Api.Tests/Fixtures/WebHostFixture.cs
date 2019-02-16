using System;
using System.IO;
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
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("IntegrationTesting")
                .ConfigureServices((context, services) => services.Configure<LimpingConfiguration>(opt => context.Configuration.GetSection("Limping")))
                .UseStartup<Startup>()
                .UseWebRoot(_webRoot)
                .ConfigureServices(services => services.AddScoped<DatabaseFixture>());
            Server = new TestServer(webHostBuilder);
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
