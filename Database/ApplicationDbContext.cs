using api_banco.Entities;
using Microsoft.EntityFrameworkCore;

namespace api_banco.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<BankTransaction> BankTransactions { get; set; }

    }
}
