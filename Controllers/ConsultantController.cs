using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;
using UniBankingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace UniBankingSystem.Controllers
{
    [Authorize(Roles = "Consultant")]
    public class ConsultantController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IRepositoryWrapper wrapper;

        public ConsultantController(UserManager<AppUser> _userManager, IRepositoryWrapper wrapper)
        {
            userManager = _userManager;
            this.wrapper = wrapper;
        }


        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> Index()
        {
            List<AppUser> lstUsers = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "User"))
                    lstUsers.Add(user);
            }

            var totalUsers = lstUsers.Count;
            var totalTransactions = (await wrapper.Transaction.GetAllAsync()).Count();
            var totalBankAccounts = (await wrapper.BankAccount.GetAllAsync()).Count();

            return View(new ConsultantViewModel
            {
                appUsers = lstUsers.AsQueryable(),
                TotalUsers = totalUsers,
                TotalTransactions = totalTransactions,
                TotalBankAccounts = totalBankAccounts
            });
        }


        public async Task<IActionResult> ViewAllLogins()
        {
        
                var allLogins = await wrapper.Logins.GetAllAsync();

                return View(new CconsultantViewModel
                {
                  
                    LoginSessions = allLogins
                                   
                                      .OrderBy(l => l.TimeStamp)
                });
            

        }

        [HttpGet]
        public async Task<IActionResult> DepositWithdraw(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return View(new ConsultantDepositModel
                {
                    AccountNumber = user.AccountNumber,
                    UserEmail = user.Email,
                });
            }

            // If user is not found, redirect to the consultant dashboard with an error message
            TempData["Message"] = "User not found.";
            return RedirectToAction("Index", "Consultant");
        }

        [HttpPost]
        public async Task<IActionResult> DepositWithdraw(ConsultantDepositModel model, string action)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.UserEmail);
                if (user != null)
                {
                    var userBankAcc = (await wrapper.BankAccount.GetAllAsync())
                                     .FirstOrDefault(bc => bc.UserEmail == user.Email);

                    if (userBankAcc != null)
                    {
                        // Check if the action is deposit or withdraw
                        if (action.ToLower() == "deposit")
                        {
                            userBankAcc.Balance += model.Amount;
                        }
                        else if (action.ToLower() == "withdraw")
                        {
                            if (userBankAcc.Balance - model.Amount < -50) // Assuming -50 as overdraft limit
                            {
                                ModelState.AddModelError("", "User has insufficient balance.");
                                return View(model);
                            }
                            userBankAcc.Balance -= model.Amount;
                        }

                        // Update the account balance and record the transaction
                        await wrapper.BankAccount.UpdateAsync(userBankAcc);
                        var transaction = new Transaction
                        {
                            Amount = model.Amount,
                            UserEmail = model.UserEmail,
                            Reference = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action)} cash in account",
                            BankAccountIdReceiver = int.Parse(user.AccountNumber),
                            BankAccountIdSender = 0,
                            TransactionDate = DateTime.Now
                        };
                        await wrapper.Transaction.AddAsync(transaction);
                        wrapper.SaveChanges();

                        TempData["Message"] = $"Successfully {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action)}ed {model.Amount} to/from the account.";
                        return RedirectToAction("Index", "Consultant");
                    }

                    ModelState.AddModelError("", "Bank account not found.");
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> ConsultantDeleteUser(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await userManager.DeleteAsync(user);
                if (results.Succeeded)
                {
                    return RedirectToAction("Index", "Consultant");
                }
                return View();
            }
            return View();
        }
        public async Task<IActionResult> ConsultantUpdateUser(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return View(new ConsultantUpdateUserModel
                {
                    AccountNumber = user.AccountNumber,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    IDNumber = user.IDnumber,
                    Lastname = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConsultantUpdateUser(ConsultantUpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.LastName = model.Lastname;
                    user.DateOfBirth = model.DateOfBirth;
                    var result = await userManager.UpdateAsync(user);
                    Message = "Updated User Details\n";
                    if (result.Succeeded)
                    {
                        if (model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                        {
                            var passResults = await userManager.RemovePasswordAsync(user);
                            if (passResults.Succeeded)
                            {
                                if ((await userManager.AddPasswordAsync(user, model.Password)).Succeeded)
                                {
                                    Message += "Successfully updated password";
                                }
                                else
                                {
                                    Message += "Error updating password...Skipping process";
                                }
                            }
                        }
                        return RedirectToAction("Index", "Consultant");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Could not find user, please contact system admin");
                    Message = "Could not find user, please contact system admin";
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult TransferFundsBetweenUsers()
        {
            return View(new ConsultantTransferFundsModel());
        }
        public async Task<IActionResult> ViewUserTransactions(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var transactions = (await wrapper.Transaction.GetAllAsync())
                                    .Where(t => t.UserEmail == user.Email)
                                    .OrderByDescending(t => t.TransactionDate)
                                    .Select(t => new TransactionDetails
                                    {
                                        TransactionDate = t.TransactionDate,
                                        Amount = t.Amount,
                                        TransactionType = t.Reference, // assuming the reference describes the type
                                        Reference = t.Reference,
                                        ReceiverAccountNumber = t.BankAccountIdReceiver.ToString(),
                                        SenderAccountNumber = t.BankAccountIdSender.ToString()
                                    })
                                    .ToList();

                return View(new ConsultantTransactionHistoryViewModel
                {
                    Transactions = transactions,
                    UserEmail = user.Email,
                    AccountNumber = user.AccountNumber // assuming AccountNumber is part of the user entity
                });
            }

            Message = "User not found.";
            return RedirectToAction("Index", "Consultant");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUserAccount(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(10); // Temporarily blocking account
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    Message = $"User {user.Email} has been blocked.";
                    return RedirectToAction("Index", "Consultant");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            Message = "User not found.";
            return RedirectToAction("Index", "Consultant");
        }


        [HttpPost]
        public async Task<IActionResult> TransferFundsBetweenUsers(ConsultantTransferFundsModel model)
        {
            if (ModelState.IsValid)
            {
                var sender = await userManager.FindByEmailAsync(model.SenderEmail);
                var receiver = await userManager.FindByEmailAsync(model.ReceiverEmail);

                if (sender != null && receiver != null)
                {
                    var senderAccount = (await wrapper.BankAccount.GetAllAsync())
                                            .FirstOrDefault(bc => bc.UserEmail == sender.Email);
                    var receiverAccount = (await wrapper.BankAccount.GetAllAsync())
                                            .FirstOrDefault(bc => bc.UserEmail == receiver.Email);

                    if (senderAccount != null && receiverAccount != null && senderAccount.Balance >= model.Amount)
                    {
                        senderAccount.Balance -= model.Amount;
                        receiverAccount.Balance += model.Amount;

                        await wrapper.BankAccount.UpdateAsync(senderAccount);
                        await wrapper.BankAccount.UpdateAsync(receiverAccount);

                        var transaction = new Transaction
                        {
                            Amount = model.Amount,
                            UserEmail = model.SenderEmail,
                            BankAccountIdReceiver = receiverAccount.Id,
                            BankAccountIdSender = senderAccount.Id,
                            TransactionDate = DateTime.Now,
                            Reference = "Fund Transfer"
                        };

                        await wrapper.Transaction.AddAsync(transaction);
                        wrapper.SaveChanges();

                        Message = "Funds transferred successfully.";
                        return RedirectToAction("Index", "Consultant");
                    }
                    ModelState.AddModelError("", "Transfer failed. Please check the accounts and balance.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid sender or receiver email.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport()
        {
            try
            {
                List<string> data = new List<string>();
                var reportContent = $"Banking Application\n{DateTime.Now:yyyyMMddHHmmss}\n\n" +
                    $"***Users\\Clients***\n" +
                    $"=====================\n" +
                    $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                var report = userManager.Users;
                foreach (var u in report)
                {
                    if (await userManager.IsInRoleAsync(u, "User"))
                    {
                        data.Add($"{u.AccountNumber}\t{u.FirstName}\t{u.LastName}\t{u.Email}\t{u.StudentStaffNumber}\n");
                    }
                }
                reportContent += string.Join('\n', data.ToArray());


                reportContent += $"\n***All Transactions***\n" +
                                 $"==========================\n" +
                    $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                var transactions = await wrapper.Transaction.GetAllAsync();
                reportContent += string.Join('\n', transactions.Select(u => $"{u.UserEmail}\t{u.Amount}\t{u.BankAccountIdReceiver}\t{u.BankAccountIdSender}\n").ToArray());

                var contentBytes = Encoding.UTF8.GetBytes(reportContent);
                var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.txt";

                return File(contentBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating report: {ex.Message}");
            }
        }
    }
}