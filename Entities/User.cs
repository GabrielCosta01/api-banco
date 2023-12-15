using api_banco.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCryptNet = BCrypt.Net.BCrypt;

namespace api_banco.Entities
{
    public class User
    {
        private string _password;
        public User()
        {
            Id = Guid.NewGuid();
            NumberAccount = Guid.NewGuid();
            Created_At = DateTime.UtcNow;
            IsDeleted = false;
        }

        public Guid Id { get; set; }

        public Guid NumberAccount { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [UniqueEmail(ErrorMessage = "Email already exists")]
        public string Email { get; set; }

        [Required]
        public string Password
        {
            get { return _password; }
            set
            {
                if (!string.IsNullOrEmpty(value))_password = BCryptNet.HashPassword(value);
            }
        }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set;}

        public bool IsDeleted { get; set; }

        public virtual ICollection<BankTransaction> Transactions { get; set; }

        public decimal AccountBalance => CalculateBalance();

        public decimal CalculateBalance()
        {
            decimal balance = 0;

            if (Transactions != null)
            {
                foreach (var transaction in Transactions)
                {
                    if (transaction.TransactionType == "deposit")
                    {
                        balance += transaction.Amount;
                    }
                    else if (transaction.TransactionType == "withdrawal")
                    {
                        balance -= transaction.Amount;
                    }
                    else if (transaction.TransactionType == "transfer" && transaction.RecipientId != Id)
                    {
                        balance -= transaction.Amount;
                    }
                    else if (transaction.TransactionType == "transfer" && transaction.RecipientId == Id)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            if (dbContext != null)
            {
                var existingUser = dbContext.Users.SingleOrDefault(u => u.Email == (string)value);

                if (existingUser != null)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
