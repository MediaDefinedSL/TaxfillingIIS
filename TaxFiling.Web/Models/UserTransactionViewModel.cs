namespace TaxFiling.Web.Models
{
    public class UserTransactionViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OnePayTransactionId { get; set; } // OnePayTransactionId
        public int TransactionStatus { get; set; }        // Approved / Failed
        public string Currency { get; set; }
        public decimal TransactionAmount { get; set; }
        public string OnePayPaidOn { get; set; }
        public string UserId { get; set; }
        public int? PackageId { get; set; }
        public DateTime TransactionCreatedDate { get; set; }
        public string UserFullName { get; set; }
        public string PackageName { get; set; }

    }
}
