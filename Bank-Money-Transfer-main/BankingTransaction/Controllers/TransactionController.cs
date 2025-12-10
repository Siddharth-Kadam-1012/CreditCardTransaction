using BankingTransaction.Data.DTO;
using BankingTransaction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        
        [HttpGet("get-transactions/{accountNumber}")]
        public async Task<IActionResult> GetTransactions(long accountNumber)
        {
            var result = await _transactionService.GetTransactionsByAccountNumberAsync(accountNumber);

            if (!result.Success)
                return BadRequest(result.Message);

            if (result.Data == null || result.Data.Count == 0)
                return NotFound("No transactions found for this account");

            return Ok(result.Data);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var response = await _transactionService.TransferAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
