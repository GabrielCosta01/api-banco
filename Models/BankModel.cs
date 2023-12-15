using System.ComponentModel.DataAnnotations;

namespace api_banco.Models
{
    public class TransactionRequestModel
    {
        [Required]
        public decimal AmountOfTransaction { get; set; }

        public string Description { get; set; }

    }

    public class DepositTransactionRequestModel
    {
        [Required]
        public decimal AmountOfDeposit { get; set; }

        public string Description { get; set; }
    }

    public class WithDrawTransactionRequestModel
    {
        [Required]
        public decimal AmountOfWithdraw { get; set; }

        public string Description { get; set; }
    }
    public class WithdrawTransactionRequestModel
    {
        public decimal AmountOfWithdraw { get; set; }
        public string Description { get; set; }
    }
}
