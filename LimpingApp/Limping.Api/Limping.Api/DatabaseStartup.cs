using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Limping.Api.Configurations;
using Limping.Api.Models;
using Limping.Api.Services;
using Limping.Api.Services.Lifetimes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Limping.Api
{
    public class DatabaseStartup : ITransientService
    {
        private readonly LimpingDbContext _context;
        private readonly IOptions<LimpingConfiguration> _configuration;
        private readonly DataMigrationService _seedDataService;

        public DatabaseStartup(LimpingDbContext context, IOptions<LimpingConfiguration> configuration, DataMigrationService seedDataService)
        {
            _context = context;
            _configuration = configuration;
            _seedDataService = seedDataService;
        }

        public DatabaseStartup Migrate()
        {
            _context.Database
                .Migrate();
            _context.SaveChanges();
            return this;
        }

        public Task ApplyDataMigrations()
        {
            return _seedDataService.ApplyDataMigrations();
        }
    }
}
