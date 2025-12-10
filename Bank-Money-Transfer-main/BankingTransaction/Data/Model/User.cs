using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingTransaction.Data.Model
{
    public class User
    {
        //    [Required]
        //    [MaxLength(20)]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string FirstName { get; set; }

        [MaxLength(10)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(20)]
        [EmailAddress]
        public string Email { get; set; }

      
        [Required]
        public string Password { get; set; }

        [Required]
        // Store account number as numeric 10-digit value
        public long AccountNumber { get; set; }

        // use decimal for money
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; } = AccountStatus.Active;

        // Optional: created/modified timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
    public enum AccountStatus
    {
        Inactive = 0,
        Active = 1,
        Closed = 2,
        Suspended = 3
    }

}
