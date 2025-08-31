using Microsoft.AspNetCore.Mvc;
using TaxFiling.Business.Interfaces;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTransactionsController : ControllerBase
    {
        private readonly IUserTransactionsRepository _userTransactionsRepository;
        private readonly IConfiguration _configuration;

        public UserTransactionsController(IUserTransactionsRepository userTransactionsRepository, IConfiguration configuration)
        {
            _userTransactionsRepository = userTransactionsRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Save a new UserTransaction and update OrderNumber
        /// </summary>
        /// <param name="transaction">Transaction data (initial fields)</param>
        /// <returns>Generated OrderId</returns>
        [HttpPost("SaveTransaction")]
        public async Task<IActionResult> SaveTransaction([FromBody] UserTransactions transaction)
        {
            if (transaction == null)
                return BadRequest("Transaction data is required.");

            try
            {
                // Save transaction and update OrderNumber
                int orderId = await _userTransactionsRepository.SaveUserTransaction(transaction);

                // Return generated OrderId
                return Ok(new { OrderId = orderId });
            }
            catch
            {
                return StatusCode(500, "An error occurred while saving the transaction.");
            }
        }

        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] UserTransactions transaction)
        {
            if (transaction == null || transaction.OrderId <= 0)
                return BadRequest("Invalid transaction data.");

            try
            {
                await _userTransactionsRepository.UpdatePaymentDetailsAsync(
                    transaction.OrderId,
                    transaction.OnePayTransactionId,
                   transaction.OnePayStatus,
                    transaction.OnePayPaidOn,
                    transaction.OnePayTransactionRequestDatetime,
                    transaction.PackageId
                );

                return Ok(new { Message = "Transaction updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating transaction: {ex.Message}");
            }
        }

        [HttpGet("GetTransactionById/{orderId}")]
        public async Task<ActionResult<UserTransactionViewDto>> GetTransactionById(int orderId)
        {
            var transaction = await _userTransactionsRepository.GetTransactionByIdAsync(orderId);

            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }
    }
}
