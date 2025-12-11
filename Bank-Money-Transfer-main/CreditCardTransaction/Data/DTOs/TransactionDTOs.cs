using CreditCardTransaction.Data.Model;

namespace CreditCardTransaction.Data.DTOs
{
    public class TransactionDTOs
    {
        public class CreateTransactionDto
        {
            public decimal Amount { get; set; }
            public TransactionCategory Category { get; set; }
            public int? Pin { get; set; }
        }

        public class TransactionDto
        {
            public int Id { get; set; }
            public int CardNumber { get; set; }
            public decimal Amount { get; set; }
            public TransactionCategory Category { get; set; }

            public decimal RewardsEarned { get; set; }
            public DateTime TransactionDate { get; set; }
        }
    }
}
