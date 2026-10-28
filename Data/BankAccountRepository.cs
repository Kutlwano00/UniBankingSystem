using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;
using UniBankingSystem.Models.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace UniBankingSystem.Data
{
    public class BankAccountRepository : RepositoryBase<BankAccount>, IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
