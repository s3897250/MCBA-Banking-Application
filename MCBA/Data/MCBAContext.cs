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

            // Configure one-to-many relationship for source account
            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountNumber)
                .OnDelete(DeleteBehavior.Cascade); // Or use .Restrict based on


            // Configure one-to-many relationship for destination account
            modelBuilder.Entity<Account>()
                .HasMany(a => a.DestinationTransactions)
                .WithOne(t => t.DestinationAccount)
                .HasForeignKey(t => t.DestinationAccountNumber)
                .OnDelete(DeleteBehavior.ClientSetNull); // Prevent cascade delete for the destination account

        }
    }
}
