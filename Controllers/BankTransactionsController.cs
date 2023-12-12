using api_banco.Database;
using api_banco.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace api_banco.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class BankTransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BankTransactionsController( ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost("send/{id}")]
        public IActionResult SendMoneyTo(BankTransactionRequestModel bodyRequest, Guid id) {
            Console.WriteLine("Entrou no controler");
            var authorizationHeader = HttpContext.Request.Headers.Authorization;
            if (!StringValues.IsNullOrEmpty(authorizationHeader) && authorizationHeader.ToString().Contains("Bearer"))
            {
                Console.WriteLine("Entrou no IF DE AUTHORIZATION");
                string token = authorizationHeader.ToString();
                token = token.Replace("Bearer", "");
                string userIdSendler = ExtractUserIdFromToken(token);

                if (userIdSendler == null) return BadRequest("token incorrect");

                var userRecipient = _dbContext.Users.SingleOrDefault(user => user.Id == id);

                if (userRecipient == null) return BadRequest("Recipient not exist");

                var userFounded = _dbContext.Users.SingleOrDefault(user => user.Id.ToString() == userIdSendler);

                if (userFounded == null) return BadRequest("User not exist");

                if (userFounded.Accountbalance! >= bodyRequest.Amount)
                {
                    return BadRequest("The user does not have enough value to carry out the transaction");
                }

                userFounded.Accountbalance -= bodyRequest.Amount;
                userRecipient.Accountbalance += bodyRequest.Amount;

                _dbContext.SaveChanges();

            }

            return BadRequest("Problem with authentication token");
        }

        private string ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

                    if (userIdClaim != null)
                    {
                        return userIdClaim.Value;
                    }
                }
            }

            return null;
        }

    }
}
