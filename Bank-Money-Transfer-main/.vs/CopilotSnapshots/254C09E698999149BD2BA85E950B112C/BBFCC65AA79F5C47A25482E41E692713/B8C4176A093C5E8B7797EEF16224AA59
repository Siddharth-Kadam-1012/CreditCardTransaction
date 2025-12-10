using System.ComponentModel.DataAnnotations;

namespace BankingTransaction.Data.ViewModel
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "First Name is Required!")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Invalid Last Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        [StringLength(10 ,MinimumLength = 1, ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [StringLength(20)] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required!")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        // Exactly 10 characters (digits). We'll validate with regex in service too.
        [Required(ErrorMessage = "Account Number is Required!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Account Number")]
        public string AccountNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Initial balance must be non-negative.")]
        public decimal InitialBalance { get; set; } = 0m;
    }
}
