using UniBankingSystem.Models;
using UniBankingSystem.Models.ViewModels.Admin;

namespace UniBankingSystem.Data.Interfaces
{
    public interface IUserRepository : IRepositoryBase<AppUser>
    {
        Task<List<UserViewModel>> GetAllUsersAndBankAccount();
    }
}
