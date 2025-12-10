namespace CreditCardTransaction.Data.DTOs
{
    public class CreditCardDTOs
    {
    }
    public class CreateCreditCardDto
    {
        // CardNumber will be generated server-side, not provided by client.
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal CreditLimit { get; set; }
        public int Pin { get; set; }
        //public decimal Outstanding { get; set; }
    }
    public class UpdateCreditCardDto
    {
        // CardNumber is not updatable here; if you want to allow it, add it with checks.
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        //public int? Pin { get; set; }
        //public decimal? CreditLimit { get; set; }
    }
    public class CreditCardDto
    {
        public int Id { get; set; }
        public long CardNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal Outstanding { get; set; }
        public DateTime BillingDate { get; set; }
    }
}
