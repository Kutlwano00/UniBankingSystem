using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using UniBankingSystem.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using UniBankingSystem.Models;
using UniBankingSystem.Models.ViewModels.Admin;
using UniBankingSystem.Models.ViewModels;

namespace UniBankingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepositoryWrapper _wrapper;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IRepositoryWrapper wrapper , UserManager<AppUser> userManager )
        {
            _wrapper = wrapper;
            _userManager = userManager;
        }
       
        

        [TempData]
        public string Message { get; set; }

       
       


        

        public async Task<IActionResult> ConsultantDeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await _userManager.DeleteAsync(user);
                if (results.Succeeded)
                {
                    return RedirectToAction("Index", "Consultant");
                }
                return View();
            }
            return View();
        }

      

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.FirstName + model.LastName + "@gmail.ac.za",
                    PhoneNumber = model.PhoneNumber,
                    LastName = model.LastName,
                    IDnumber = model.IDNumber,
                   


                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var bankAccount = new BankAccount
                    {
                        AccountNumber = GenerateRandomAccountNumber(),
                        Balance = 150m,
                        UserEmail = model.Email
                    };
                    await _wrapper.BankAccount.AddAsync(bankAccount);
                  

                    Message = "User created successfully";
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> UpdateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var userBankAccount = (await _wrapper.BankAccount.GetAllAsync())
                                        .FirstOrDefault(bc => bc.UserEmail == user.Email);

                return View(new UpdateUserModel
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IDNumber = user.IDnumber,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    AccountNumber = userBankAccount?.AccountNumber,

                });
            }
            return RedirectToAction("Index");
        }
        public static string GenerateRandomAccountNumber()
        {
            Random random = new Random();
            string accountNumber = "";

            for (int i = 0; i < 16; i++)
            {
                accountNumber += random.Next(0, 10);  // Generates a digit from 0 to 9
            }

            return accountNumber;
        }

        public async Task<IActionResult> Index()
        {
            var userCount = (await _userManager.GetUsersInRoleAsync("User")).Count();
            var consultantCount = (await _userManager.GetUsersInRoleAsync("Consultant")).Count();
            var transactionCount = (await _wrapper.Transaction.GetAllAsync()).Count();
            var orderCount = (await _wrapper.Transaction.GetAllAsync()).Count(); // Assuming orders are tracked in the same repository

            var model = new AdminDashboardViewModel
            {
                UserCount = userCount,
                ConsultantCount = consultantCount,
                TransactionCount = transactionCount,
                OrderCount = orderCount
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IDnumber = model.IDNumber;
                    user.DateOfBirth = model.DateOfBirth;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                       
                        var bankAccount = (await _wrapper.BankAccount.GetAllAsync())
                                          .FirstOrDefault(bc => bc.UserEmail == user.Email);
                        if (bankAccount != null)
                        {
                            bankAccount.AccountNumber = model.AccountNumber;
                         
                            await _wrapper.BankAccount.UpdateAsync(bankAccount);
                        }

                        if (!string.IsNullOrEmpty(model.Password) && model.Password == model.ConfirmPassword)
                        {
                            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                            if (removePasswordResult.Succeeded)
                            {
                                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                                if (addPasswordResult.Succeeded)
                                {
                                    Message = "User and password updated successfully";
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error updating password");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error removing old password");
                            }
                        }

                        return RedirectToAction("Users");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var bankAccount = (await _wrapper.BankAccount.GetAllAsync())
                                        .FirstOrDefault(bc => bc.UserEmail == user.Email);
                    if (bankAccount != null)
                    {
                        await _wrapper.BankAccount.RemoveAsync(bankAccount.Id);
                    }
                    return RedirectToAction("ManageUsers");
                }
            }
            return View();
        }

      
        public async Task<IActionResult> ManageConsultants()
        {
            var consultants = (await _userManager.GetUsersInRoleAsync("Consultant")).ToList();
            return View(consultants);
        }

      

        public async Task<IActionResult> ViewUsers()
        {
            var users = await _wrapper.AppUser.GetAllUsersAndBankAccount();
            return View(users);
        }

        
        public async Task<IActionResult> ViewOrders()
        {
            var orders = await _wrapper.Transaction.GetAllAsync();
            return View(orders);
        }

        

        
    }



}
