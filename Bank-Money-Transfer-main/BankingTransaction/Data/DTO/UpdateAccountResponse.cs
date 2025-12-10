using System.ComponentModel.DataAnnotations;

namespace BankingTransaction.Data.DTO
{
    public class UpdateAccountResponse
    {
       
        public int id { get; set; }
        public string FirstName { get; set; }

         public string LastName { get; set; }

         public string Email { get; set; }
        public string Password { get; set; }

        public string message { get; set; }
    }
}
