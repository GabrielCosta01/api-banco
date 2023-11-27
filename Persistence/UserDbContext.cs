using api_banco.Entities;

namespace api_banco.Persistence
{
    public class UserDbContext
    {
        public List<User> Users {  get; set; }
        public UserDbContext() {
            Users = new List<User>();
        }
    }
}
