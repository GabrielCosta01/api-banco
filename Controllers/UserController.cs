using api_banco.Entities;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using api_banco.Models;
using api_banco.Database;

namespace api_banco.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public UserController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult CreateUserPost(UserCreationModel userCreationModel) {

            var existUser = _context.Users.SingleOrDefault(user => user.Email == userCreationModel.Email);
            
            if (existUser != null)return BadRequest("User alredy exist");

            var newUser = new User
            {
                Name = userCreationModel.Name,
                Email = userCreationModel.Email,
                Password = userCreationModel.Password,
                Accountbalance = userCreationModel.Accountbalance,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var userModel = new UserModel
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Password = newUser.Password,
                NumberAccount = newUser.NumberAccount,
                Accountbalance = newUser.Accountbalance,
                Email = newUser.Email,
            };

            return CreatedAtAction(nameof(GetById), new { id = userModel.Id }, userModel);
        }

        [HttpPost("/login")]
        public IActionResult SessionUserPost(UserLoginModel userBody)
        {
            var findUser = _context.Users.SingleOrDefault(user => user.Email == userBody.Email);

            if (findUser == null)
            {
                return BadRequest("User not found");
            }

            var passwordMatch = BCryptNet.Verify(userBody.Password, findUser.Password);

            if (!passwordMatch) {
                return BadRequest("Incorret password");
            }

            var token = _tokenService.GenerateToken(findUser.Id.ToString());

            return Ok(new { Token = token});
        }

        [HttpGet("{id}")] public IActionResult GetById(Guid id)
        {
            BankTransactionsController bankTransactionsController = new BankTransactionsController(_context);

            string authorization = HttpContext.Request.Headers.Authorization;

            if (authorization == null)return BadRequest(new {message = "Problem with authentication token"});

            string userOwnerForToken = bankTransactionsController.ExtractUserIdFromToken(authorization);
            var userOwnerForId = _context.Users.SingleOrDefault(user => user.Id == id);

            if (userOwnerForId.Id.ToString() != userOwnerForToken) return BadRequest(new { message = "You dont can make this action" });

            var userFind = _context.Users.SingleOrDefault(d => d.Id == id);

            if(userFind == null)return NotFound(new {message="User not found"});

            return Ok(userFind);
        }

        [HttpPatch("update")] public IActionResult PatchUserOwner(UserModelUpdate userFields)
        {
            BankTransactionsController bankTransactionsController = new BankTransactionsController(_context);
            string authorization = HttpContext.Request.Headers.Authorization;
            
            if (authorization == null) return BadRequest(new { message = "Problem with authentication token" });

            string userId = bankTransactionsController.ExtractUserIdFromToken(authorization);
            var userUpdate = _context.Users.SingleOrDefault(user => user.Id.ToString() == userId);

            if (!string.IsNullOrEmpty(userFields.Email) &&
                _context.Users.Any(u => u.Id != userUpdate.Id && u.Email == userFields.Email))
            {
                return BadRequest(new { message = "Email is already in use by another user" });
            }

            userUpdate.Name = userFields.Name;
            userUpdate.Email = userFields.Email;
            userUpdate.Password = userFields.Password;
            userUpdate.Updated_At = DateTime.UtcNow;

            _context.SaveChanges();
            
            return Ok(userUpdate);
        }

        [HttpDelete("delete")] public IActionResult DeleteUser()
        {
            BankTransactionsController bankTransactionsController = new BankTransactionsController(_context);

            string authorization = HttpContext.Request.Headers.Authorization;
            if (authorization == null) return BadRequest(new { message = "Problem with authentication token" });

            string userId = bankTransactionsController.ExtractUserIdFromToken(authorization);
            var user = _context.Users.SingleOrDefault(user => user.Id.ToString() == userId);

            user.IsDeleted = true;

            _context.SaveChanges();
            
            return Ok(authorization);
        }
    }
}
