using CreditCardTransaction.Data.DTOs;
using CreditCardTransaction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentServices _service;

        public PaymentController(PaymentServices service)
        {
            _service = service;
        }

        [HttpGet("by-card/{cardNumber:int}")]
        public async Task<IActionResult> GetByCard(int cardNumber, CancellationToken ct)
        {
            var payments = await _service.GetPaymentsByCardAsync(cardNumber, ct);
            return Ok(payments);
        }


        [HttpPost("pay-bill/{cardNumber:int}")]
        public async Task<IActionResult> Pay(int cardNumber, [FromBody] CreatePaymentDto dto, CancellationToken ct)
        {
            try
            {
                var payment = await _service.MakePaymentAsync(cardNumber, dto, ct);
                return Ok(payment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // log
                return StatusCode(500, new { message = "An error occurred processing the payment." });
            }
        }
    }
}
