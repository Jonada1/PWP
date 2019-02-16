using System;
using System.Threading.Tasks;
using Limping.Api.Models;
using Limping.Api.Services;

namespace Limping.Api.Tests.Fixtures
{
    // If we need to enable parallelism for database, we should generate a new database for each of the threads
    public class DatabaseFixture : IDisposable
    {
        private readonly DataMigrationService _dataMigrationService;
        private readonly bool _manageDatabase;

        public LimpingDbContext Context { get; }

        public DatabaseFixture(LimpingDbContext context, DataMigrationService dataMigrationService)
        {
            _dataMigrationService = dataMigrationService;
            Context = context;

            var database = context.Database;

            _manageDatabase = true;
            if (_manageDatabase)
            {
                database.EnsureDeleted();
            }

            database.EnsureCreated();
        }

        public Task ApplyDataMigrations()
        {
            return _dataMigrationService.ApplyDataMigrations();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_manageDatabase)
                {
                    Context.Database
                        .EnsureDeleted();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
