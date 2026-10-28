using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

namespace UniBankingSystem.Data
{
    public class ReviewRepository : RepositoryBase<FeedBack>, IReviewRepository
    {
        public ReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
