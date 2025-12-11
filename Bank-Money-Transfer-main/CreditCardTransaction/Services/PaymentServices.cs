using CreditCardTransaction.Data;
using CreditCardTransaction.Data.DTOs;
using CreditCardTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CreditCardTransaction.Services
{
    public class PaymentServices
    {
        private readonly AppDbContext _context;

        public PaymentServices(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<PaymentDto>> GetPaymentsByCardAsync(int cardNumber, CancellationToken ct = default)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.CardNumber == cardNumber)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    CardNumber = p.CardNumber,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Method = p.Method
                })
                .ToListAsync(ct);
        }


        public async Task<PaymentDto> MakePaymentAsync(int cardNumber, CreatePaymentDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            // Validate PIN if provided
            if (dto.Pin.HasValue && (dto.Pin.Value < 1000 || dto.Pin.Value > 9999))
                throw new ArgumentException("Pin must be 4 digits.");

            // locate card (with tracking because we'll update it)
            var card = await _context.CreditCard.FirstOrDefaultAsync(c => c.CardNumber == cardNumber, ct);
            if (card == null) throw new ArgumentException("Card not found.");

            // if pin present, validate matches stored
            if (dto.Pin.HasValue && card.Pin != dto.Pin.Value)
                throw new ArgumentException("Invalid PIN.");

            // cannot pay more than outstanding (optionally allow overpay to create negative outstanding)
            if (dto.Amount > card.Outstanding)
            {
                if(card.Outstanding == 0)
                {
                    throw new ArgumentException("You don't have any outstanding balance left!");
                }
                throw new ArgumentException($"Payment amount cannot exceed outstanding balance rupees {card.Outstanding}.");
            }

            using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var payment = new Payment
                {
                    CardNumber = card.CardNumber,
                    Amount = dto.Amount,
                    PaymentDate = DateTime.UtcNow,
                    Method = dto.Method,
                };

                _context.Payments.Add(payment);

                // update outstanding
                card.Outstanding -= dto.Amount;
                _context.CreditCard.Update(card);

                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return new PaymentDto
                {
                    Id = payment.Id,
                    CardNumber = payment.CardNumber,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    Method = payment.Method
                };
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

    }

}
