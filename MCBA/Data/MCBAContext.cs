using Microsoft.EntityFrameworkCore;
using MCBA.Models;

namespace MCBA.Data
{
    public class MCBAContext : DbContext
    {
        public MCBAContext(DbContextOptions<MCBAContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<Payee> Payees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Set check constraints (cannot be expressed with data annotations).
            modelBuilder.Entity<Login>().ToTable(b =>
            {
                b.HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");
                b.HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 94");
            });

            modelBuilder.Entity<Account>().ToTable(b => b.HasCheckConstraint("CH_Account_Balance", "Balance >= 0"));
            modelBuilder.Entity<Transaction>().ToTable(b => b.HasCheckConstraint("CH_Transaction_Amount", "Amount > 0"));

        }
    }
}
