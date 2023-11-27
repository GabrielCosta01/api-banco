using System;
using api_banco.Entities;
using System.ComponentModel.DataAnnotations;

namespace api_banco.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        public string Email{ get; set; }

        [Required]
        public string Password { get; set; }
    }
}
