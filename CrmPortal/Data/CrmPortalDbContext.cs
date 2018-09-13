using CrmPortal.Model;
using Microsoft.EntityFrameworkCore;

namespace CrmPortal.Data
{
    public class CrmPortalDbContext : DbContext
    {
        public CrmPortalDbContext()
        {
        }

        public CrmPortalDbContext(DbContextOptions options)
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
    }
}
