using BankingTransaction.Data;
using BankingTransaction.Data.DTO;
using BankingTransaction.Data.Model;
using BankingTransaction.Data.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BankingTransaction.Services
{
    public class AccountService
    {
        private readonly AppDbContext _context;
        // Common regexes
        private static readonly Regex AccountRegex = new Regex(@"^\d{10}$", RegexOptions.Compiled);
        // Password policy: at least 8 chars, contain uppercase, lowercase, digit, special (adjust as needed)
        private static readonly Regex PasswordPolicyRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex =
    new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        private UserDTO toUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                AccountNumber = user.AccountNumber,
                Balance = user.Balance,
                Status = user.Status.ToString()
            };
        }
        public async Task<UserDTO> GetAccountByAccountNo(string accountNo)
        {
            var user = await _context.Users
                .Where(u => u.AccountNumber == accountNo)
                .Select(u => new User
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    AccountNumber = u.AccountNumber,
                    Balance = u.Balance,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();
            if (user == null)
                return null;
            return toUserDTO(user);
        }
        
        public async Task<bool> DeleteAccountAsync(string accountNo)
        {
            var user = await _context.Users
                .Where(u => u.AccountNumber == accountNo)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UpdateAccountResponse> UpdateUserAsync(string accountNo,UpdateAccountRequest request)
        {
            var user = await _context.Users
              .Where(u => u.AccountNumber == accountNo).FirstOrDefaultAsync();
            if (user == null)
            {
                return new UpdateAccountResponse
                {
                    message = "User Not Found"
                };
            }
            else if (!EmailRegex.IsMatch(request.Email) && request.Email != null)
            {
                return new UpdateAccountResponse
                {
                    message = "Invalid Email!"
                };
            }
            else if (request.FirstName.Length <= 0 || request.FirstName.Length >= 20)
            {
                return new UpdateAccountResponse
                {
                    message = "Invalid First Name"
                };
              
            }
            else if (request.LastName.Length <= 0 || request.LastName.Length >= 20)
            {
                return new UpdateAccountResponse
                {
                    message = "Invalid Last Name"
                };
            }
            else if (!PasswordPolicyRegex.IsMatch(request.Password))
            {
                return new UpdateAccountResponse
                {
                    message = "Invalid Password! Password must be at least 8 characters and include uppercase, lowercase, digit and special character."
                };
            }

            if (user.Email != request.Email)
            {
                bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
                if (emailExists)
                {
                    return new UpdateAccountResponse
                    {
                        message = "Email Already Exists"
                    };
                   
                }


            }
            // Update fields
            user.FirstName = request.FirstName ;
            user.LastName = request.LastName ;
           
            user.Email = request.Email;

            user.Password = request.Password;
            await _context.SaveChangesAsync();
            return new UpdateAccountResponse
            {
                id = user.Id,
                FirstName = user.FirstName 
                , LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                
                message = "Details Updated Successfully"
            };

        }


        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            String message = "Account Created Successfully";
            bool isAccountCredValid = true;

            // Basic null check
            if (request.FirstName == null || request.LastName == null || request.Email == null || request.Password == null || request.AccountNumber == null)
            {
                message = "Incomplte Data! Null Value Found";
                isAccountCredValid = false;
            }

            // Validate account number format
            else if (request.FirstName.Length <= 0 || request.FirstName.Length >= 20)
            {
                message = "Invalid First Name";
                isAccountCredValid = false;
            }
            else if (request.LastName.Length <= 0 || request.LastName.Length >= 20)
            {
                message = "Invalid Last Name";
                isAccountCredValid = false;
            }
            else if (!EmailRegex.IsMatch(request.Email))
            {
                message = "Invalid Email";
                isAccountCredValid = false;
            }


            else if (!AccountRegex.IsMatch(request.AccountNumber))
            {
                message = "Invalid Account Number";
                isAccountCredValid = false;
            }

            else if (!PasswordPolicyRegex.IsMatch(request.Password))
            {
                message = "Invalid Password! Password must be at least 8 characters and include uppercase, lowercase, digit and special character.";
                isAccountCredValid = false;
            }


            bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());

            // Check account number uniqueness
            bool accExists = await _context.Users.AnyAsync(u => u.AccountNumber == request.AccountNumber);

            if (emailExists)
            {
                message = "Email Already Exists";
                isAccountCredValid = false;

            }


            else if (accExists)
            {
                message = "Account Already Exists";
                isAccountCredValid = false;
            }
            
            // Validate initial balance non-negative (DTO has Range, but re-check)
            if (request.InitialBalance < 0 || request.InitialBalance  > 10000)
            {
                message = "Initial Balance Cannot Be Negative";
                isAccountCredValid = false;
            }
            if (isAccountCredValid == false)
            {
                return new CreateUserResponse
                {
                    message = message
                };
            }
            else
            {
                try
                {
                    // Create User entity
                    var user = new BankingTransaction.Data.Model.User
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email.Trim(),
                        AccountNumber = request.AccountNumber,
                        Balance = request.InitialBalance,
                        CreatedAt = DateTime.UtcNow,
                        Status = BankingTransaction.Data.Model.AccountStatus.Active,
                        Password = request.Password
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    // Prepare response
                    return new CreateUserResponse
                    {
                        Id = user.Id,
                        Name = user.FirstName +" "+ user.LastName,
                        Email = user.Email,
                        AccountNumber = user.AccountNumber,
                        Balance = user.Balance,
                        Status = user.Status.ToString(),
                        CreatedAt = user.CreatedAt,
                        message = "Account Created Successfully"
                    };
                }
                catch (Exception e)
                {
                    return new CreateUserResponse
                    {
                        message = "Acount Details Cannot be stored in Database" + e.Message
                    };
                }


            }

            

            
        }


     

    }
}
