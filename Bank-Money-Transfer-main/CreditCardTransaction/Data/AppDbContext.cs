using CreditCardTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace CreditCardTransaction.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCard { get; set; }

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
        }
    }
}
