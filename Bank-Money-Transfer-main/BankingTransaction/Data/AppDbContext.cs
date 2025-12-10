using BankingTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace BankingTransaction.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(u => u.FirstName)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(u => u.LastName)
                      .HasMaxLength(20);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.HasIndex(u => u.Email)
                      .IsUnique(); // ensure unique emails

                entity.Property(u => u.Password)
                      .IsRequired();
                entity.Property(a => a.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(11);

                entity.Property(a => a.Balance)
                      .HasColumnType("decimal(18,2)");
                // unique constraint for account number
                entity.HasIndex(a => a.AccountNumber)
                      .IsUnique();

            });

        }
    }
}
