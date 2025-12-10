namespace BankingTransaction.Data.DTO
{
    public class TransactionDTOs
    {
    }
    public class TransferRequest
    {
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; } // optional
    }

    public class TransferResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TransactionDto Transaction { get; set; }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CompletedAt { get; set; }
        public string FailureReason { get; set; }
    }
    public class TransactionHistoryResponse
    {
        public IEnumerable<TransactionDto> Transactions { get; set; }
    }

}

