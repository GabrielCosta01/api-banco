using api_banco.Database;
using System;
using System.ComponentModel.DataAnnotations;
using BCryptNet = BCrypt.Net.BCrypt;

namespace api_banco.Entities
{
    public class User
    {
        private string _password;
        public User()
        {
            Accountbalance = 0;
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
            set { _password = BCryptNet.HashPassword(value); } 
        }

        public decimal Accountbalance { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set;}

        public bool IsDeleted { get; set; }
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
