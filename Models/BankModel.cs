using System.ComponentModel.DataAnnotations;

namespace api_banco.Models
{
    public class BankTransactionRequestModel
    {
        [Required]
        public decimal Amount { get; set; }

        public string description { get; set; }

    }
}
