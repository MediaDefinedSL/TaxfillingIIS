using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Domain.Dtos
{
    public class UserTransactionsDto
    {
        public int OrderId { get; set; }   // Primary Key, included for identification
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public int PackageId { get; set; }
        public decimal? TransactionAmount { get; set; }
        public int? TransactionStatus { get; set; }
        public string Currency { get; set; }
        public DateTime? TransactionCreatedDate { get; set; }
        public string OnePayTransactionId { get; set; }
        public string OnePayTransactionRequestDatetime { get; set; }
        public bool OnePayStatus { get; set; }
        public string OnePayPaidOn { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Packages Package { get; set; } = null!;

    }
}
