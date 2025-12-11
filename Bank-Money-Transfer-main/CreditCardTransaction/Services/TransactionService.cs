using CreditCardTransaction.Data;
using CreditCardTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;
using static CreditCardTransaction.Data.DTOs.TransactionDTOs;

namespace CreditCardTransaction.Services
{
    public class TransactionService
    {

        private readonly AppDbContext _context;

        // Reward rates by category (as percentage of amount)
        private readonly Dictionary<TransactionCategory, decimal> _rewardRates = new()
        {
            { TransactionCategory.Dining, 0.05m },       // 5%
            { TransactionCategory.Shopping, 0.02m },     // 2%
            { TransactionCategory.Travel, 0.03m },       // 3%
            { TransactionCategory.Fuel, 0.018m },         // 1%
            { TransactionCategory.Electronics, 0.015m },  // 1%
            { TransactionCategory.Groceries, 0.02m },    // 2%
            { TransactionCategory.Other, 0.01m }         // 1%
        };
        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        // Create transaction (purchase): increases outstanding and assigns rewards
        public async Task<TransactionDto> CreateTransactionAsync(int cardNumber, CreateTransactionDto dto, CancellationToken ct = default)
        {

            //Validation Start
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
            // find card with tracking because we'll update it
            var card = await _context.CreditCard.FirstOrDefaultAsync(c => c.CardNumber == cardNumber, ct);
            if (card == null) throw new ArgumentException("Card not found.");

            // optional PIN validation
            if (dto.Pin.HasValue)
            {
                if (dto.Pin.Value < 1000 || dto.Pin.Value > 9999)
                    throw new ArgumentException("Pin must be a 4-digit number.");
                if (card.Pin != dto.Pin.Value)
                    throw new ArgumentException("Invalid PIN.");
            }

            // credit limit check: ensure outstanding + amount <= credit limit
            var newOutstanding = card.Outstanding + dto.Amount;
            if (newOutstanding > card.CreditLimit)
                throw new ArgumentException("Transaction would exceed credit limit.");


            // Validation END

            // compute reward
            decimal rate = _rewardRates.GetValueOrDefault(dto.Category, 0.01m);
            decimal rewards = Math.Round(dto.Amount * rate, 2);

            using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var entity = new Transaction
                {
                    CardNumber = card.CardNumber,
                    Amount = dto.Amount,
                    Category = dto.Category,
                    RewardsEarned = rewards,
                    TransactionDate = DateTime.UtcNow
                };

                _context.Transactions.Add(entity);

                // update card
                card.Outstanding = newOutstanding;
                card.RewardsBalance += rewards;
                _context.CreditCard.Update(card);

                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return new TransactionDto
                {
                    Id = entity.Id,
                    CardNumber = entity.CardNumber,
                    Amount = entity.Amount,
                    Category = entity.Category,
                    RewardsEarned = entity.RewardsEarned,
                    TransactionDate = entity.TransactionDate
                };
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        // get transactions for a card (optionally with paging)
        public async Task<IEnumerable<TransactionDto>> GetTransactionsByCardAsync(int cardNumber, CancellationToken ct = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.CardNumber == cardNumber)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    CardNumber = t.CardNumber,
                    Amount = t.Amount,
                    Category = t.Category,
                    RewardsEarned = t.RewardsEarned,
                    TransactionDate = t.TransactionDate
                }).ToListAsync(ct);
        }
    }
}
