using Microsoft.EntityFrameworkCore;

namespace Limping.Api.Models
{
    public class LimpingDbContext: DbContext
    {
        public LimpingDbContext(DbContextOptions<LimpingDbContext> options)
           : base(options) { }
        public DbSet<DataMigration> DataMigrations { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<LimpingTest> LimpingTests { get; set; }
        public DbSet<TestAnalysis> TestAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.Email)
                .IsUnique();
            modelBuilder.Entity<AppUser>()
                .HasIndex(x => x.UserName)
                .IsUnique();
            modelBuilder.Entity<LimpingTest>()
                .HasOne(lt => lt.TestAnalysis)
                .WithOne(ta => ta.LimpingTest)
                .HasForeignKey<TestAnalysis>(ta => ta.LimpingTestId);
            modelBuilder.Entity<LimpingTest>()
                .HasOne(lt => lt.AppUser)
                .WithMany(usr => usr.LimpingTests);
        }
    }
}
