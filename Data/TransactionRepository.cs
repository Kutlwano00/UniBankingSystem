using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

namespace UniBankingSystem.Data
{

    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository

    {
        public TransactionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
