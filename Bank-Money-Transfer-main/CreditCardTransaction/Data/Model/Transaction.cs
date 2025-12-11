namespace CreditCardTransaction.Data.Model
{
    public class Transaction
    {
        public int Id { get; set; }

        // Store card number redundantly for easy queries/audit
        public int CardNumber { get; set; }

        // Positive amount for purchase
        public decimal Amount { get; set; }

        // Category enum
        public TransactionCategory Category { get; set; }

        public decimal RewardsEarned { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        //public bool IsReversed { get; set; } = false;
    }
    public enum TransactionCategory
    {
        Dining = 1,
        Shopping = 2,
        Travel = 3,
        Fuel = 4,
        Electronics = 5,
        Groceries = 6,
        Other = 7
    }

}
