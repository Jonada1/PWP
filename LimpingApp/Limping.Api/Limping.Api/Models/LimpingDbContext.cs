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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LimpingTest>()
                .HasOne(lt => lt.TestAnalysis)
                .WithOne(ta => ta.LimpingTest)
                .HasForeignKey<TestAnalysis>(ta => ta.LimpingTestId);
        }
    }
}
