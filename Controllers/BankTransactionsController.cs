using api_banco.Database;
using api_banco.Models;
using Microsoft.AspNetCore.Mvc;
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

        
        [HttpPost("send/{id}")]
        public IActionResult SendMoneyTo(BankTransactionRequestModel bodyRequest, Guid id) {

            String authorizationHeader = HttpContext.Request.Headers.Authorization;

            if (authorizationHeader == null) return BadRequest(new {message = "Problem with authentication token" });

            string token = authorizationHeader.ToString();
            token = token.Replace("Bearer", "");
            string userIdSendler = ExtractUserIdFromToken(token);

            if (userIdSendler == null)return BadRequest(new { message = "Token incorrect" });

            var userRecipient = _dbContext.Users.SingleOrDefault(user => user.Id == id);

            if (userRecipient == null)return BadRequest(new { message = "Recipient not exist" });

            var userFounded = _dbContext.Users.SingleOrDefault(user => user.Id.ToString() == userIdSendler);

            if (userFounded == null)return BadRequest(new { message = "User not exist" });

            if (userFounded.Accountbalance < bodyRequest.Amount)
            {
                return BadRequest(new
                {
                    message = "The user does not have enough value to carry out the transaction",
                    AccountBalance = userFounded.Accountbalance
                });
            }

            userFounded.Accountbalance -= bodyRequest.Amount;
            userRecipient.Accountbalance += bodyRequest.Amount;



            _dbContext.SaveChanges();

            return Ok(new { message = "Transaction successful" });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var userIdClaim = jwtToken.Claims.FirstOrDefault((claim) => claim.Type == "unique_name");
                    
                    if (userIdClaim != null)return userIdClaim.Value;
                }
            }

            return null;
        }

    }
}
