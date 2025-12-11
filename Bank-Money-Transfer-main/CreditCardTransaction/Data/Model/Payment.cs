namespace CreditCardTransaction.Data.Model
{
    public class Payment
    {
        public int Id { get; set; }

        public int CardNumber { get; set; }

        // Payment amount (positive)
        public decimal Amount { get; set; }

        // When payment was made
        public DateTime PaymentDate { get; set; }

        // optional: method (Cash, Online, NEFT, Card etc.)
        public string? Method { get; set; }
    }
}
