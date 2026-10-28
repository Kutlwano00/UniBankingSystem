using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

namespace UniBankingSystem.Data
{
    public class ChargesRepository : RepositoryBase<Charges>, IChargesRepository

    {
        public ChargesRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
