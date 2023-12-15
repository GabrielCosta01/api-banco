using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("SenderId")]
        public Guid SenderId { get; set; }
        public virtual User Sender { get; set; }

        [Required]
        [ForeignKey("RecipientId")]
        public Guid RecipientId { get; set; }
        public virtual User Recipient {  get; set; }

        [Required]
        public decimal Amount { get; set;}

        public DateTime SendingAt { get; set; }

        public string Description { get; set; }

        [Required]
        public string TransactionType { get; set; }

    }
}
