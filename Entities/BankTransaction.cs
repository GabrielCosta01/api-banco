using System.ComponentModel.DataAnnotations;

namespace api_banco.Entities
{
    public class BankTransaction
    {
        public BankTransaction(){
            Id = Guid.NewGuid();
            SendingAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Recipient { get; set; }

        [Required]
        public decimal Amount { get; set;}

        public DateTime SendingAt { get; set; }

        public string description { get; set; }
    }
}
