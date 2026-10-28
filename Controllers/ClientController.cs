using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

using UniBankingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UniBankingSystem.Controllers
{
    public class ClientController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        private readonly UserManager<AppUser> _userManager;

        public ClientController(IRepositoryWrapper repo, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = new UpdateProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                Lastname = user.LastName,
                AccountNumber = user.AccountNumber,
                IDNumber = user.IDnumber,
                DateOfBirth = user.DateOfBirth,
               Userrole = user.UserRole
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                if (user != null)
                {
                    // Update only editable fields
                    user.PhoneNumber = model.PhoneNumber;
                    user.FirstName = model.FirstName;
                    user.LastName = model.Lastname;

                    // Save changes
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Profile updated successfully.";
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }


        public async Task<IActionResult> Index()
        {
            // Retrieve the username of the currently logged-in user
            var username = User.Identity.Name;

            // Find the user based on their username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve user's bank accounts
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var userBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            // Retrieve user's notifications
            var allNotifications = await _repo.Notification.GetAllAsync();
            var userNotifications = allNotifications.Where(n => n.UserEmail == user.Email).ToList();

            // Mark all notifications as read
            foreach (var notification in userNotifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    await _repo.Notification.UpdateAsync(notification);
                }
            }

          
            var allTransactions = await _repo.Transaction.GetAllAsync();
            var userTransactions = allTransactions.Where(t => t.UserEmail == user.Email).ToList();

  
            var model = new UserDashboardViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IDNumber = user.IDnumber,
                UserRole = user.UserRole,
                LastName = user.LastName + " " + user.FirstName,
                BankAccount = new BankAccountViewModel
                {
                    AccountNumber = userBankAccount?.AccountNumber,
                    Balance = userBankAccount?.Balance ?? 0
                },
                Notifications = userNotifications,
                Transactions = userTransactions
            };

            return View(model);
        }

        public async Task<IActionResult> Message()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var allNotifications = await _repo.Notification.GetAllAsync();
            var userNotifications = allNotifications.Where(n => n.UserEmail == user.Email).Take(4).ToList();

            foreach (var notification in userNotifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    await _repo.Notification.UpdateAsync(notification);
                }
            }

            return View(userNotifications);
        }

        [HttpGet]
        public async Task<IActionResult> Transaction()
        {

            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {

                return NotFound();
            }
            var allTransaction = await _repo.Transaction.GetAllAsync();

            var userTransaction = allTransaction.Where(n => n.UserEmail == user.Email).ToList();

            return View(userTransaction);
        }

        


       

        [HttpGet]
        public IActionResult AddRating()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRatingAsync(FeedBack feedback)
        {
            if (ModelState.IsValid)
            {
                // Set default values
                feedback.dateTime = DateTime.Now; // Set current date and time
                feedback.Rate = 5; // Set default rating value, e.g., 5
                feedback.Comment = feedback.Comment ?? "No comment provided"; // Set default comment if none provided
                feedback.UserEmail = User.Identity.Name; // Set the current user's email

                await _repo.Review.AddAsync(feedback);
                return RedirectToAction("Index", "Client");
            }
            return View(feedback);
        }









        public async Task<bool> TransferMoney(string senderAccountNumber, string receiverAccountNumber, decimal amount)
        {
            
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();

            // Find sender and receiver bank accounts
            var senderBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == senderAccountNumber);
            var receiverBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == receiverAccountNumber);

            // Check if both accounts exist
            if (senderBankAccount == null || receiverBankAccount == null)
            {
                return false;
            }


            if (senderBankAccount.Balance < amount)
            {
                return false; // Insufficient balance
            }


            senderBankAccount.Balance -= amount;
            receiverBankAccount.Balance += amount;


            await _repo.BankAccount.UpdateAsync(senderBankAccount);
            await _repo.BankAccount.UpdateAsync(receiverBankAccount);

          
            var senderNotification = new Notification
            {
                Message = $"You have sent {amount:C} to account {receiverBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = senderBankAccount.UserEmail
            };
            var receiverNotification = new Notification
            {
                Message = $"You have received {amount:C} from account {senderBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = receiverBankAccount.UserEmail
            };

            await _repo.Notification.AddAsync(senderNotification);
            await _repo.Notification.AddAsync(receiverNotification);



            return true;
        }

        [HttpGet]
        public async Task<IActionResult> TransferMoneyView()
        {
           
            var username = User.Identity.Name;

        
            var user = await _userManager.FindByNameAsync(username);

          
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();

           
            var mainBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

       
            var viewModel = new MoneyTransferViewModel
            {
                SenderBankAccountNumber = mainBankAccount?.AccountNumber, 
                AvailableBalance = mainBankAccount?.Balance ?? 0, 
                ReceiverBankAccountNumber = "", 
                Accounts = allBankAccounts
                    .Where(b => b.UserEmail == user.Email) 
                    .Select(b => new AccountViewModel
                    {
                        AccountNumber = b.AccountNumber,
                        OwnerName = user.UserName 
                    })
                    .ToList()
            };

       
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> TransferMoneyView(MoneyTransferViewModel model)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            // Get all bank accounts for the user
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            string senderAccountNumber = mainBankAccount.AccountNumber;
            string receiverAccountNumber = model.ReceiverBankAccountNumber;
            decimal amount = model.Amount;


            bool transferSuccess = await TransferMoney(senderAccountNumber, receiverAccountNumber, amount);

            if (transferSuccess)
            {
                return RedirectToAction("TransferSuccess", new { amount = amount, receiverAccount = receiverAccountNumber });
            }
            else
            {
                      return View("NotFound");
                
            }
        }

        public IActionResult TransferSuccess(decimal amount, string senderAccount, string receiverAccount)
        {
            ViewBag.Amount = amount;
            ViewBag.SenderAccount = senderAccount;
            ViewBag.ReceiverAccount = receiverAccount;
            ViewBag.TransferDate = DateTime.Now;

            return View();
        }








    }

}
