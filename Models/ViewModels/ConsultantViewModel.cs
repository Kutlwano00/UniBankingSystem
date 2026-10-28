namespace UniBankingSystem.Models.ViewModels
{
    public class ConsultantViewModel
    {
        public IQueryable<AppUser> appUsers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalBankAccounts { get; set; }
    }

    public class CconsultantViewModel
    {
        public AppUser SelectedUser { get; set; }
        public IEnumerable<LoginSessions> LoginSessions { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int UserCount { get; set; }
        public int ConsultantCount { get; set; }
        public int TransactionCount { get; set; }
        public int OrderCount { get; set; }
    }

    public class ConsultantDepositModel
    {
        public string UserEmail { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}