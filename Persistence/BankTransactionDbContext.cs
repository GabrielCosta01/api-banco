using api_banco.Entities;

namespace api_banco.Persistence
{
    public class BankTransactionDbContext
    {
        public List<BankTransaction> BankTransactions { get; set; }
        public BankTransactionDbContext() {
            BankTransactions = new List<BankTransaction>();
        }

    }
}
