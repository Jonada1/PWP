using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Limping.Api.DataMigrations;
using Limping.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Limping.Api.Services
{
    public class DataMigrationService
    {
        private readonly LimpingDbContext _context;

        public IEnumerable<BaseDataMigration> AllMigrations { get; }

        public DataMigrationService(LimpingDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;

            // These must be considered immutable, ie. if you want to delete seed data, please add a new one at the end.
            // We could have dates in names similar to EF migrations, but I think it would require more displine than this does.
            AllMigrations = new[]
                {
                    typeof(AddDefaultUser),
                }
                .Select(type => serviceProvider.GetRequiredService(type) as BaseDataMigration)
                .ToList();
        }

        // Npgsql does not seem to properly support HasData, so use this as a workaround:
        // https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/issues/367
        public async Task ApplyDataMigrations()
        {
            var appliedMigrations = await _context.DataMigrations
                .ToDictionaryAsync(dataMigration => dataMigration.Id, _ => true);

            var newMigrations = AllMigrations
                .Where(migration => !appliedMigrations.ContainsKey(migration.Id));
            foreach (var migration in newMigrations)
            {
                await migration.Apply();
                await _context.DataMigrations
                    .AddAsync(new DataMigration(migration.Id));
                await _context.SaveChangesAsync();
            }
        }
    }
}
