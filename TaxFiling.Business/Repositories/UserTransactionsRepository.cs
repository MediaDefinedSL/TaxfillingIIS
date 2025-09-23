using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;


namespace TaxFiling.Business.Repositories
{
    public class UserTransactionsRepository : IUserTransactionsRepository
    {
        private readonly Context _context;
        private readonly ILogger<PackagesRepository> _logger;
        private readonly IConfiguration _configuration;

        public UserTransactionsRepository(Context context, ILogger<PackagesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Save initial UserTransaction and return the generated OrderId
        /// </summary>
        public async Task<int> SaveUserTransaction(UserTransactions transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            try
            {
                // Set creation date if not already set
                if (!transaction.TransactionCreatedDate.HasValue)
                    transaction.TransactionCreatedDate = DateTime.UtcNow;

                // Only initial columns
                var entity = new UserTransactions
                {
                    //OrderNumber = transaction.OrderNumber,
                    UserId = transaction.UserId,
                    PackageId = transaction.PackageId,
                    TransactionAmount = transaction.TransactionAmount,
                    TransactionStatus = transaction.TransactionStatus,
                    Currency = transaction.Currency,
                    TransactionCreatedDate = transaction.TransactionCreatedDate
                };

                await _context.UserTransactions.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Initial UserTransaction saved with OrderId: {OrderId}", entity.OrderId);

                entity.OrderNumber = $"ORDTAX-{entity.OrderId:0000}";
                _context.UserTransactions.Update(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("UserTransaction saved and OrderNumber updated: {OrderNumber}", entity.OrderNumber);

                // Return the generated OrderId
                return entity.OrderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving initial UserTransaction");
                throw;
            }
        }

        public async Task UpdatePaymentDetailsAsync(int orderId, string onePayTransactionId, bool onePayStatus, string onePayPaidOn, string onePayRequestedDate, int PackageId)
        {
            var transaction = await _context.UserTransactions
                .Include(t => t.User)  // make sure navigation property exists
                .FirstOrDefaultAsync(t => t.OrderId == orderId);

            if (transaction == null)
                throw new Exception("Transaction not found");

            transaction.OnePayTransactionId = onePayTransactionId;
            transaction.OnePayStatus = onePayStatus;
            transaction.OnePayPaidOn = onePayPaidOn;
            transaction.OnePayTransactionRequestDatetime = onePayRequestedDate;
            if(onePayStatus && onePayPaidOn != null)
                transaction.TransactionStatus = 1;
            else
                transaction.TransactionStatus = 0;

            // Update user's is_active_payment if payment succeeded
            if (onePayStatus && transaction.User != null)
            {
                transaction.User.IsActivePayment = 1;
                transaction.User.PackageId = PackageId;
                //if (PackageId == 4 || PackageId == 5 || PackageId == 6)
                //transaction.User.taxAssistedUserUploadDocsStatus = 0;
                if (PackageId == 4 || PackageId == 5 || PackageId == 6)
                {
                    int currentYear = DateTime.UtcNow.Year;
                    var userDocStatus = await _context.UserUploadDocStatus
                        .FirstOrDefaultAsync(u => u.UserId == transaction.User.UserId.ToString() && u.Year == currentYear);

                    if (userDocStatus != null)
                    {
                        // Update existing row
                        userDocStatus.DocStatus = 0;
                        userDocStatus.IsPersonalInfoCompleted = 0;
                        userDocStatus.IsIncomeTaxCreditsCompleted = 0;
                        userDocStatus.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create a new row for the current year
                        userDocStatus = new UserUploadDocStatus
                        {
                            UserId = transaction.User.UserId.ToString(),
                            Year = currentYear,
                            DocStatus = 0,
                            IsPersonalInfoCompleted = 0,
                            IsIncomeTaxCreditsCompleted = 0,
                            UpdatedDate = DateTime.UtcNow
                        };
                        _context.UserUploadDocStatus.Add(userDocStatus);
                    }
                }
            }
            //_context.UserTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<UserTransactionViewDto?> GetTransactionByIdAsync(int orderId)
        {
            var transaction = await _context.UserTransactions
               .Include(t => t.User)
               .Include(t => t.Package)
               .Where(t => t.OrderId == orderId)
               .Select(t => new UserTransactionViewDto
               {
                   OrderId = t.OrderId,
                   OrderNumber = t.OrderNumber,
                   UserFullName = $"{t.User.FirstName} {t.User.LastName}",
                   PackageName = t.Package.Name,
                   TransactionAmount = t.TransactionAmount,
                   Currency = t.Currency,
                   TransactionCreatedDate = t.TransactionCreatedDate,
                   OnePayTransactionId = t.OnePayTransactionId,                   
                   OnePayPaidOn = t.OnePayPaidOn,
                   TransactionStatus = t.TransactionStatus
               })
               .FirstOrDefaultAsync();  // Include related Package

            return transaction;
        }




    }
}
