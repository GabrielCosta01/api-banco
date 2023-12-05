using System;
using api_banco.Entities;
using System.ComponentModel.DataAnnotations;

namespace api_banco.Models
{
    public class UserLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserCreationModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public decimal Accountbalance { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class UserModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        public Guid NumberAccount { get; set; }

        [Required]
        public decimal Accountbalance { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
