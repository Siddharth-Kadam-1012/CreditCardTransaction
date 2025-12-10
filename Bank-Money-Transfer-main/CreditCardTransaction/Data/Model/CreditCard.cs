namespace CreditCardTransaction.Data.Model
{
    public class CreditCard
    {
        public int Id { get; set; }

        
        public int CardNumber { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public int Pin { get; set; }

        public decimal CreditLimit { get; set; }

        public decimal Outstanding { get; set; }

        public DateTime BillingDate { get; set; }
    }
}
