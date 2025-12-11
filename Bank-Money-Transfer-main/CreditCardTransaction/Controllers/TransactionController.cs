using CreditCardTransaction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CreditCardTransaction.Data.DTOs.TransactionDTOs;

namespace CreditCardTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _service;

        public TransactionController(TransactionService service)
        {
            _service = service;
        }
        // POST /api/Transactions/{cardNumber}/create
        [HttpPost("create-transaction/{cardNumber:int}")]
        public async Task<IActionResult> Create(int cardNumber, [FromBody] CreateTransactionDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _service.CreateTransactionAsync(cardNumber, dto, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred creating transaction." });
            }
        }

        // GET /api/Transactions/by-card/{cardNumber}
        [HttpGet("get-transaction/{cardNumber:int}")]
        public async Task<IActionResult> GetByCard(int cardNumber, CancellationToken ct)
        {
            var items = await _service.GetTransactionsByCardAsync(cardNumber, ct);
            return Ok(items);
        }

    }
}
