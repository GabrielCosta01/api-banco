using api_banco.Entities;
using api_banco.Persistence;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using api_banco.Middleware;
using api_banco.Models;

namespace api_banco.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly TokenService _tokenService;

        public UserController(UserDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult CreateUserPost(User user)
        {
            _context.Users.Add(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPost("/login")]
        public IActionResult SessionUserPost(UserModel user)
        {
            // RECEBO EMAIL E SENHA
            // ENCONTRO O USUARIO PELO EMAIL
            // VERIFICO SE A SENHA VINDA DO BANCO DE DADOS BATE COM A SENHA QUE RECEBI COM A REQUISIÇÃO
            // SE FOREM IGUAIS, RETORNO UM TOKEN DE ACESSO PARA O USUARIO
            // SE NÃO, FALO QUE EMAIL OU SENHA ESTÃO INCORRETOS "Email or password incorrect"

            var findUser = _context.Users.SingleOrDefault(user => user.Email == user.Email);

            if (findUser == null)
            {
                return BadRequest("User not found");
            }

            var passwordMatch = BCryptNet.Verify(user.Password, findUser.Password);

            if (!passwordMatch) {
                return BadRequest("Incorret password");
            }

            var token = _tokenService.GenerateToken(findUser.Id, "SECRET_KEY");

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
