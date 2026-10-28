using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniBankingSystem.Data.Interfaces;
using UniBankingSystem.Models;

namespace UniBankingSystem.Data
{
    public class LoginRepository : RepositoryBase<LoginSessions>, ILoginRepository

    {
        public LoginRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}