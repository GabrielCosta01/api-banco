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
            
            if (existUser != null)
            {
                return BadRequest("User alredy exist");
            }

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
            var userFind = _context.Users.SingleOrDefault(d => d.Id == id);

            if(userFind == null)
            {
                return NotFound();
            }

            return Ok(userFind);
        }


    }
}
