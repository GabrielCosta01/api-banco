using api_banco.Entities;
using api_banco.Models;

namespace api_banco.Helpers
{
    public class UserMapper
    {
        public static User MapToUser(UserModel userModel) {
            return new User
            {
                Id = userModel.Id,
                Name = userModel.Name,
                Password = userModel.Password,
                NumberAccount = userModel.NumberAccount,
                Accountbalance = userModel.Accountbalance,
                Email = userModel.Email
            };
        }
    }
}
