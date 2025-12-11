using CreditCardTransaction.Data;
using CreditCardTransaction.Data.DTOs;
using CreditCardTransaction.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CreditCardTransaction.Services
{
    public class CreditCardServices
    {
        private readonly AppDbContext _context;
        private const int CardGenerationRetries = 10;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public CreditCardServices(AppDbContext db)
        {
            _context = db;
        }

        private void ValidateCreateDto(CreateCreditCardDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.CustomerName)) throw new ArgumentException("CustomerName is required.");
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail) || !EmailRegex.IsMatch(dto.CustomerEmail)) throw new ArgumentException("Valid CustomerEmail is required.");
            if (dto.CreditLimit < 20000) throw new ArgumentException("CreditLimit must be >= 20000.");
            if (dto.Pin < 1000 || dto.Pin > 9999) throw new ArgumentException("Pin must be a 4-digit number between 1000 and 9999.");

            //if (dto.Outstanding < 0) throw new ArgumentException("Outstanding must be >= 0.");
            //if (dto.Outstanding > dto.CreditLimit) throw new ArgumentException("Outstanding cannot exceed CreditLimit.");

        }
        private void ValidateUpdateDto(UpdateCreditCardDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail) || !EmailRegex.IsMatch(dto.CustomerEmail)) throw new ArgumentException("Valid CustomerEmail is required.");
            //if (dto.CreditLimit < 20000) throw new ArgumentException("CreditLimit must be >= 20000.");
            //if (dto.Outstanding < 0) throw new ArgumentException("Outstanding must be >= 0.");
            //if (dto.Outstanding > dto.CreditLimit) throw new ArgumentException("Outstanding cannot exceed CreditLimit.");
      
        }



        public async Task<CreditCardDto?> GetByCardNumberAsync(int cardNumber, CancellationToken ct = default)
        {
            var entity = await _context.CreditCard
                .FirstOrDefaultAsync(x => x.CardNumber == cardNumber, ct);

            if (entity == null)
                return null;

            return ToDto(entity);
        }

        public async Task<CreditCardDto> CreateAsync(CreateCreditCardDto dto, CancellationToken ct = default)
        {
            ValidateCreateDto(dto);

          
            // Generate unique card number
            int cardNumber = await GenerateUniqueCardNumberAsync(ct);

            var entity = new CreditCard
            {
                CardNumber = cardNumber,
                CustomerName = dto.CustomerName!.Trim(),
                CustomerEmail = dto.CustomerEmail!.Trim(),
                CreditLimit = dto.CreditLimit,
                Outstanding = 0,
                BillingDate = DateTime.UtcNow.Date.AddMonths(1),
                Pin = dto.Pin,
                RewardsBalance = 0
            };

            // Additional server-side checks
            //if (entity.Outstanding < 0) throw new ArgumentException("Outstanding cannot be negative.");
            if (entity.CreditLimit < 0) throw new ArgumentException("CreditLimit cannot be negative.");
            //if (entity.Outstanding > entity.CreditLimit) throw new ArgumentException("Outstanding cannot exceed CreditLimit.");

            _context.CreditCard.Add(entity);
            await _context.SaveChangesAsync(ct);

            return ToDto(entity);
        }

        public async Task<CreditCardDto?> UpdateAsync(int cardNumber, UpdateCreditCardDto dto, CancellationToken ct = default)
        {

            //if (dto == null) throw new ArgumentNullException(nameof(dto));

            //ValidateUpdateDto(dto);

            var entity = await _context.CreditCard
                                 .FirstOrDefaultAsync(x => x.CardNumber == cardNumber, ct);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.CustomerName))
            {

                entity.CustomerName = dto.CustomerName!.Trim();
            }

            if (EmailRegex.IsMatch(dto.CustomerEmail)) 
            {

                entity.CustomerEmail = dto.CustomerEmail!.Trim();
            }

           
            _context.CreditCard.Update(entity);
            await _context.SaveChangesAsync(ct);

            return ToDto(entity);
        }

        public async Task<bool> UpdatePinAsync(int cardNumber,int oldPin,int newPin,CancellationToken ct = default)
        {
            var entity = await _context.CreditCard
                                 .FirstOrDefaultAsync(x => x.CardNumber == cardNumber);
            if (entity == null) throw new ArgumentException("Credit Card Number Not Found!");


            if (entity.Pin != oldPin) throw new ArgumentException("Invalid Old Pin");
            if (newPin < 1000 || newPin > 9999)
                throw new ArgumentException("Pin must be a 4-digit number between 1000 and 9999.");

            entity.Pin = newPin;
            _context.CreditCard.Update(entity);
            await _context.SaveChangesAsync(ct);
            return true;

        }

        public async Task<bool> UpdateCreditLimitAsync(int cardNumber, int newCreditLimit,int pin,CancellationToken ct = default)
        {
            var entity = await _context.CreditCard
                                 .FirstOrDefaultAsync(x => x.CardNumber == cardNumber);
            if (entity == null) throw new ArgumentException("Credit Card Number Not Found!");
            if (entity.Pin != pin) throw new ArgumentException("Invalid  Pin");

            if (newCreditLimit < 20000)
                throw new ArgumentException("CreditLimit must be >= 20000.");


            if (entity.Outstanding > entity.CreditLimit)
                throw new ArgumentException("Outstanding cannot exceed new CreditLimit.");


            entity.CreditLimit = newCreditLimit;


            _context.CreditCard.Update(entity);
            await _context.SaveChangesAsync(ct);
            return true;

        }

        public async Task<bool> DeleteAsync(int cardNumber,int pin, CancellationToken ct = default)
        {
            var entity = await _context.CreditCard
                                 .FirstOrDefaultAsync(x => x.CardNumber == cardNumber, ct);


            if (entity == null) return false;

            if (entity.Pin != pin) throw new ArgumentException("Invalid  Pin");

            _context.CreditCard.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        private static CreditCardDto ToDto(CreditCard e) =>
            new CreditCardDto
            {
                Id = e.Id,
                CardNumber = e.CardNumber,
                CustomerName = e.CustomerName,
                CustomerEmail = e.CustomerEmail,
                CreditLimit = e.CreditLimit,
                Outstanding = e.Outstanding,
                BillingDate = e.BillingDate
            };


        private async Task<int> GenerateUniqueCardNumberAsync(CancellationToken ct = default)
        {
            // We'll generate 16-digit positive numbers in the range [10^15, 10^16-1]
            // Use RNGCryptoServiceProvider / RandomNumberGenerator for secure randomness

            for (int attempt = 0; attempt < CardGenerationRetries; attempt++)
            {
                var candidate = Generate9DigitNumber();
                bool exists = await _context.CreditCard.AnyAsync(x => x.CardNumber == candidate, ct);
                if (!exists)
                    return candidate;
            }

            // If the naive attempts fail (extremely unlikely), fallback to a loop until unique
            while (true)
            {
                var candidate = Generate9DigitNumber();
                bool exists = await _context.CreditCard.AnyAsync(x => x.CardNumber == candidate, ct);
                if (!exists) return candidate;
            }
        }

        private static int Generate9DigitNumber()
        {
            // Use cryptographically secure RNG
            Span<byte> bytes = stackalloc byte[4]; // only need 4 bytes for int range
            RandomNumberGenerator.Fill(bytes);

            uint raw = BitConverter.ToUInt32(bytes);

            // 9-digit range
            const uint min = 100_000_000;   // smallest 9-digit
            const uint range = 900_000_000; // range size for 9-digit numbers

            uint candidate = min + (raw % range);

            return (int)candidate;
        }

    }
}
