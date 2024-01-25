using Microsoft.EntityFrameworkCore;
using MCBAAdminAPI.Models;

namespace MCBAAdminAPI.Data
{
    public class MCBAAdminContext : DbContext
    {
        public MCBAAdminContext(DbContextOptions<MCBAAdminContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set check constraints (cannot be expressed with data annotations).
            modelBuilder.Entity<Login>().ToTable(b =>
            {
                b.HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");
                b.HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 94");
            });
        }
    }
}
