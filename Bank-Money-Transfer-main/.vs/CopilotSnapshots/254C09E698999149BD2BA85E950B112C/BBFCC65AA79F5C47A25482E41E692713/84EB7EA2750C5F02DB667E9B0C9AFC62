using System.Transactions;

namespace BankingTransaction.Data.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; } = 0m;
        public TransactionStatus status { get; set; }
        //public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; }
        public string? FailureReason { get; set; }


    }
    public enum TransactionStatus
    {
        Failed = 0,
        Completed = 1,
        Pending = 2
    }
}
