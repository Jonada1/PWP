using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Limping.Api.Models
{
    public class LimpingDbContext: IdentityDbContext<AppUser>
    {
        public LimpingDbContext(DbContextOptions<LimpingDbContext> options)
           : base(options) { }

        public DbSet<LimpingTest> LimpingTests { get; set; }
        public DbSet<TestAnalysis> TestAnalyses { get; set; }
        public DbSet<TestComparison> TestComparisons { get; set; }
        public DbSet<LimpingTestTestComparison> LimpingTestTestComparisons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Many-to-many between Limping Test & Test Comparison
            modelBuilder.Entity<LimpingTestTestComparison>()
                .HasKey(lttc => new { lttc.LimpingTestId, lttc.TestComparisonId });
            modelBuilder.Entity<LimpingTestTestComparison>()
                .HasOne(lttc => lttc.LimpingTest)
                .WithMany(lt => lt.LimpingTestTestComparisons)
                .HasForeignKey(lttc => lttc.LimpingTestId);
            modelBuilder.Entity<LimpingTestTestComparison>()
                .HasOne(lttc => lttc.TestComparison)
                .WithMany(tc => tc.LimpingTestTestComparisons)
                .HasForeignKey(lttc => lttc.TestComparisonId);

            modelBuilder.Entity<LimpingTest>()
                .HasOne(lt => lt.TestAnalysis)
                .WithOne(ta => ta.LimpingTest)
                .HasForeignKey<TestAnalysis>(ta => ta.LimpingTestId);
        }
    }
}
