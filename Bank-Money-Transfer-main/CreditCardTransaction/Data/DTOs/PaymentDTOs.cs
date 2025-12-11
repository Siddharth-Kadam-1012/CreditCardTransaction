using CreditCardTransaction.Data.Model;

namespace CreditCardTransaction.Data.DTOs
{
    public class PaymentDTOs
    {
    }

    public class CreatePaymentDto
    {
        public decimal Amount { get; set; }
        public int? Pin { get; set; }

        public PaymentCategory Method { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public int CardNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentCategory Method { get; set; }
    }

}
