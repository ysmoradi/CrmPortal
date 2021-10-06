using Bit.Data.EntityFrameworkCore.Implementations;
using CrmPortal.Model;
using CrmPortal.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CrmPortal.Data
{
    public class CrmPortalDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CrmPortalDbContext>
    {
        public IConfiguration Configuration { get; set; }

        public CrmPortalDbContext CreateDbContext(string[] args)
        {
            Configuration ??= CrmPortalConfigurationProvider.GetConfiguration();

            return new CrmPortalDbContext(new DbContextOptionsBuilder<CrmPortalDbContext>()
                .UseSqlServer(connectionString: Configuration.GetConnectionString("AppConnectionString")).Options);
        }
    }

    public class CrmPortalDbContext : EfCoreDbContextBase
    {
        public CrmPortalDbContext(DbContextOptions<CrmPortalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.UserName)
                .IsUnique(true);
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<BlackList> BlackLists { get; set; }

        public virtual DbSet<Token> Tokens { get; set; }
    }
}
