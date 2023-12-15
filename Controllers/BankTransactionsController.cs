using api_banco.Database;
using api_banco.Entities;
using api_banco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [HttpPost("deposit/{id}")]
        public IActionResult DepositTransaction(Guid id, DepositTransactionRequestModel bodyDeposit)
        {
            String authorizationHeader = HttpContext.Request.Headers.Authorization;

            if (authorizationHeader == null) return BadRequest(new { message = "Problem with authentication token" });

            string token = authorizationHeader.ToString();
            token = token.Replace("Bearer", "");
            string userIdSender = ExtractUserIdFromToken(token);

            var userSender = _dbContext.Users.SingleOrDefault(user => user.Id.ToString() == userIdSender);

            if (userSender == null) return BadRequest(new { message = "User not exist" });

            var userRecipient = _dbContext.Users.SingleOrDefault(user => user.Id == id);

            if (userRecipient == null) return BadRequest(new { message = "Recipient not exist" });


            var transaction = new BankTransaction
            {
                SenderId = userSender.Id,
                RecipientId = userRecipient.Id,
                Amount = bodyDeposit.AmountOfDeposit,
                TransactionType = "deposit",
                Description = bodyDeposit.Description
            };

            _dbContext.BankTransactions.Add(transaction);

            _dbContext.Entry(transaction).Reference(t => t.Sender).Load();
            _dbContext.Entry(transaction).Reference(t => t.Recipient).Load();

            _dbContext.SaveChanges();

            return Ok(transaction);
        }

        [HttpPost("send/{id}")]
        public IActionResult SendMoneyTo(TransactionRequestModel bodyRequest, Guid id)
        {
            String authorizationHeader = HttpContext.Request.Headers.Authorization;

            if (authorizationHeader == null) return BadRequest(new { message = "Problem with authentication token" });

            string token = authorizationHeader.ToString();
            token = token.Replace("Bearer", "");
            string userIdSender = ExtractUserIdFromToken(token);

            if (userIdSender == null) return BadRequest(new { message = "Token incorrect" });

            var userRecipient = _dbContext.Users
                .Include(u => u.Transactions)
                .SingleOrDefault(user => user.Id == id);

            if (userRecipient == null) return BadRequest(new { message = "Recipient not exist" });

            var userSender = _dbContext.Users
                .Include(u => u.Transactions)
                .SingleOrDefault(user => user.Id.ToString() == userIdSender);

            if (userSender == null) return BadRequest(new { message = "User not exist" });

            if (userSender.AccountBalance < bodyRequest.AmountOfTransaction)
            {
                return BadRequest(new
                {
                    message = "The user does not have enough value to carry out the transaction",
                    AccountBalance = userSender.AccountBalance
                });
            }

            var transaction = new BankTransaction
            {
                SenderId = userSender.Id,
                RecipientId = userRecipient.Id,
                Amount = bodyRequest.AmountOfTransaction,
                TransactionType = "transfer",
                Description = bodyRequest.Description
            };

            _dbContext.BankTransactions.Add(transaction);

            userRecipient.CalculateBalance();
            userSender.CalculateBalance();

            _dbContext.SaveChanges();

            return Ok(new { message = "Transaction successful" });
        }

        [HttpGet("statement")]
        public IActionResult GetAccountStatement()
        {
            String authorizationHeader = HttpContext.Request.Headers.Authorization;

            if (authorizationHeader == null) return BadRequest(new { message = "Problem with authentication token" });

            string token = authorizationHeader.ToString();
            token = token.Replace("Bearer", "");
            string userOwner = ExtractUserIdFromToken(token);

            if (userOwner == null) return BadRequest(new { message = "Token incorrect" });

            var user = _dbContext.Users
                .Include(u => u.Transactions)
                .SingleOrDefault(u => u.Id.ToString() == userOwner);

            if (user == null)return BadRequest(new { message = "User not exist" });

            var accountStatement = new
            {
                UserId = user.Id,
                name = user.Name,
                AccountBalance = user.AccountBalance,
                Transactions = user.Transactions.Select(t => new
                {
                    TransactionId = t.Id,
                    Sender = t.Sender != null ? new { t.Sender.Id, t.Sender.Name } : null,
                    Recipient = t.Recipient != null ? new { t.Recipient.Id, t.Recipient.Name } : null,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    Description = t.Description,
                    SendingAt = t.SendingAt
                })
            };

            return Ok(accountStatement);
        }

        [HttpPost("withdraw")]
        public IActionResult WithdrawTransaction(WithdrawTransactionRequestModel bodyWithdraw)
        {
            String authorizationHeader = HttpContext.Request.Headers.Authorization;

            if (authorizationHeader == null) return BadRequest(new { message = "Problem with authentication token" });

            string token = authorizationHeader.ToString();
            token = token.Replace("Bearer", "");
            string userOwnerId = ExtractUserIdFromToken(token);

            var user = _dbContext.Users.Include(u => u.Transactions).SingleOrDefault(user => user.Id.ToString() == userOwnerId);

            if (user == null) return BadRequest(new { message = "User not exist" });

            if (user.AccountBalance < bodyWithdraw.AmountOfWithdraw)
            {
                return BadRequest(new
                {
                    message = "The user does not have enough balance to make the withdrawal",
                    AccountBalance = user.AccountBalance
                });
            }

            var transaction = new BankTransaction
            {
                SenderId = user.Id,
                RecipientId = user.Id,
                Amount = bodyWithdraw.AmountOfWithdraw,
                TransactionType = "withdrawal",
                Description = bodyWithdraw.Description
            };

            _dbContext.BankTransactions.Add(transaction);

            _dbContext.SaveChanges();

            return Ok(new { message = "Withdrawal successful" });
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
