using CreditCardTransaction.Data.DTOs;
using CreditCardTransaction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace CreditCardTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        private readonly CreditCardServices _service;

        public CreditCardController(CreditCardServices service)
        {
            _service = service;
        }


        // GET: api/CreditCards/by-card/123456789
        [HttpGet("get-by-card-number/{cardNumber:int}")]
        public async Task<IActionResult> GetByCardNumber(int cardNumber, CancellationToken ct)
        {
            var card = await _service.GetByCardNumberAsync(cardNumber, ct);
            if (card == null) return NotFound();
            return Ok(card);
        }

        // POST: api/CreditCards
        [HttpPost("create-account")]
        public async Task<IActionResult> Create([FromBody] CreateCreditCardDto dto, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(dto, ct);
                // return location using the GetByCardNumber route
                return CreatedAtAction(nameof(GetByCardNumber),
                    new { cardNumber = created.CardNumber }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // consider logging the exception
                return StatusCode(500, new { message = "An error occurred creating credit card account." });
            }
        }

        // PUT: api/CreditCards/by-card/123456789
        [HttpPut("update-by-card-no/{cardNumber:int}")]
        public async Task<IActionResult> Update(int cardNumber, [FromBody] UpdateCreditCardDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _service.UpdateAsync(cardNumber, dto, ct);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // consider logging the exception
                return StatusCode(500, new { message = "An error occurred updating credit card account." });
            }
        }

        // PUT: api/CreditCards/by-card/123456789
        [HttpPut("update-pin-by-card-no/{cardNumber:int}/{oldPin:int}/{newPin:int}")]
        public async Task<IActionResult> UpdatePin(int cardNumber,int oldPin,int newPin, CancellationToken ct)
        {
            try
            {
                var isPinChanged = await _service.UpdatePinAsync(cardNumber,oldPin,newPin);
                if (isPinChanged)
                {
                    return Ok("Pin Changed Successfully!");
                }
                return BadRequest("Pin Failed to change!");

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // consider logging the exception
                return StatusCode(500, new { message = "An error occurred updating credit card account." });
            }
        }
        // PUT: api/CreditCards/by-card/123456789
        [HttpPut("update-credit-limit-by-card-no/{cardNumber:int}/{newCreditLimit:int}/{Pin:int}")]
        public async Task<IActionResult> UpdateCreditLimit(int cardNumber,int newCreditLimit,int Pin, CancellationToken ct)
        {
            try
            {
                var isCreditLimitChanged = await _service.UpdateCreditLimitAsync(cardNumber, newCreditLimit, Pin);
                if (isCreditLimitChanged)
                {
                    return Ok("Credit Limit Changed Successfully!");
                }
                return BadRequest("Credit Limit Failed to change!");

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // consider logging the exception
                return StatusCode(500, new { message = "An error occurred updating credit card account." });
            }
        }

        // DELETE: api/CreditCards/by-card/123456789
        [HttpDelete("by-card/{cardNumber:int}/{Pin:int}")]
        public async Task<IActionResult> Delete(int cardNumber,int Pin, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(cardNumber, Pin, ct);
            if (!deleted) return NotFound();
            return Ok("Deleted Successfully!");
        }
    }
}
