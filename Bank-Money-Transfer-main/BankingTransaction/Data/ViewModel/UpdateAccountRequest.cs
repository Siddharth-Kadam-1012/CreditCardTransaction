using System.ComponentModel.DataAnnotations;

namespace BankingTransaction.Data.ViewModel
{
    public class UpdateAccountRequest
    {
        [Required(ErrorMessage = "First Name is Required!")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Invalid Last Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required!")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required!")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [StringLength(20)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }




    }
}
