using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

namespace UniBankingSystem.Data
{
  
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {

        public NotificationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
