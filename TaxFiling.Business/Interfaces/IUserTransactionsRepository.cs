using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces
{
    public interface IUserTransactionsRepository
    {
        Task<int> SaveUserTransaction(UserTransactions transaction);
        Task UpdatePaymentDetailsAsync(int orderId, string onePayTransactionId, bool onePayStatus, string onePayPaidOn, string onePayRequestedDate, int PackageId);

        Task<UserTransactionViewDto?> GetTransactionByIdAsync(int orderId);

    }
}
