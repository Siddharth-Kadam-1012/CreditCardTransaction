using CreditCardTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace CreditCardTransaction.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCard { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CreditCard>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.CardNumber).IsUnique(); // Unique constraint for safety
                b.Property(x => x.CustomerName).IsRequired().HasMaxLength(30);
                b.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(30);
                b.Property(x => x.CreditLimit).HasColumnType("decimal(18,2)");
                b.Property(x => x.Outstanding).HasColumnType("decimal(18,2)");

                b.Property(x => x.Pin).IsRequired();

            });
            modelBuilder.Entity<Payment>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                b.Property(p => p.PaymentDate).IsRequired();
                b.Property(p => p.Method).HasMaxLength(50);
                
            });
            modelBuilder.Entity<Transaction>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Amount).HasColumnType("decimal(18,2)");
                b.Property(t => t.RewardsEarned).HasColumnType("decimal(18,2)");
                b.Property(t => t.TransactionDate).IsRequired();
            });

        }
    }
}
