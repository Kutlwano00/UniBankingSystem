
using System.ComponentModel.DataAnnotations;

namespace UniBankingSystem.Models.ViewModels




{
    public class UserDashboardViewModel
    {
        // User details
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IDNumber { get; set; }
        public string UserRole { get; set; }
        public string LastName { get; set; }

        // Bank account details
        public BankAccountViewModel BankAccount { get; set; }

        // List of notifications
        public List<Notification> Notifications { get; set; }

        // List of transactions
        public List<Transaction> Transactions { get; set; }
    }

    public class BankAccountViewModel
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
    public class ConsultantTransactionHistoryViewModel
    {
        public string AccountNumber { get; set; }
        public string UserEmail { get; set; }
        public List<TransactionDetails> Transactions { get; set; }

        public ConsultantTransactionHistoryViewModel()
        {
            Transactions = new List<TransactionDetails>();
        }
    }

    public class TransactionDetails
    {
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } // Deposit, Withdraw, Transfer, etc.
        public string Reference { get; set; }
        public string ReceiverAccountNumber { get; set; } // For transfer-related transactions
        public string SenderAccountNumber { get; set; } // For transfer-related transactions
    }

    public class ConsultantTransferFundsModel
    {
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string Reference { get; set; }
    }

    public class BankAccountViewModela
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string BankAccountType { get; set; }
    }
    public class ConsultantBankAccountViewModel
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string UserEmail { get; set; }
    }



    public class MoneyTransferViewModel
    {
        [Required]
        public int SenderBankAccountId { get; set; }


        public int ReceiverBankAccountId { get; set; }

        [Required(ErrorMessage = "The amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than zero.")]
        public decimal Amount { get; set; }


        public string SenderBankAccountNumber { get; set; }

        [Required(ErrorMessage = "The receiver's bank account number is required.")]

        public string ReceiverBankAccountNumber { get; set; }

        public List<AccountViewModel> Accounts { get; set; }
        public decimal AvailableBalance { get; set; }
    }
    public class TransferSuccessViewModel
    {
        public decimal Amount { get; set; }
        public string SenderAccount { get; set; }
        public string ReceiverAccount { get; set; }
        public DateTime TransferDate { get; set; }
    }


public class AccountViewModel
    {
        public string AccountNumber { get; set; }
        public string OwnerName { get; set; }
    }

}
