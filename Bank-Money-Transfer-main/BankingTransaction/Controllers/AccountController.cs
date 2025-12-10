using BankingTransaction.Data.ViewModel;
using BankingTransaction.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankingTransaction.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("get-account/{accountNumber}")]
        public async Task<IActionResult> GetAccountByAccountNo(long accountNumber)
        {
            var userDTO = await _accountService.GetAccountByAccountNo(accountNumber);
            if (userDTO != null)
            {
                return Ok(userDTO);
            }
            else
            {
                return NotFound("Account not found");
            }
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateUserRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createUserResponse = await _accountService.CreateUserAsync(request);
            if (createUserResponse.message == "Account Created Successfully")
            {
                return Ok(createUserResponse);
            }
            else
            {
                return BadRequest(createUserResponse.message);
            }
        }

        [HttpPut("update-account/{accountNumber}")]
        public async Task<IActionResult> UpdateAccount(long accountNumber, [FromBody] UpdateAccountRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updateAccountResponse = await _accountService.UpdateUserAsync(accountNumber, request);
            if (updateAccountResponse.message == "Details Updated Successfully")
            {
                return Ok(updateAccountResponse);
            }
            else
            {
                return BadRequest(updateAccountResponse.message);
            }
        }
        [HttpDelete("delete-account/{accountNumber}")]
        public async Task<IActionResult> DeleteAccount(long accountNumber)
        {
            var deleteAccountResponse = await _accountService.DeleteAccountAsync(accountNumber);
            if (deleteAccountResponse)
            {
                return Ok("Account Deleted Successfully");
            }
            else
            {
                return BadRequest("Failed to Delete Account! Check the account number");
            }

        }
    }
}
