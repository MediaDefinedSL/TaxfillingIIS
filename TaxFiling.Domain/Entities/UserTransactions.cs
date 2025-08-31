using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities
{
    [Table("tblUserTransactions")]
    public class UserTransactions
    {
        [Column("OrderId")]
        [Key]
        public int OrderId { get; set; }
        [Column("OrderNumber")]
        public string OrderNumber { get; set; }
        [Column("UserId")]
        public Guid UserId { get; set; }
        [Column("PackageId")]
        public int PackageId { get; set; }
        [Column("TransactionStatus")]
        public int TransactionStatus { get; set; }
        [Column("TransactionAmount")]
        public decimal? TransactionAmount { get; set; }
        [Column("Currency")]
        public string Currency { get; set; }
        [Column("TransactionCreatedDate")]
        public DateTime? TransactionCreatedDate { get; set; }
        [Column("OnePayTransactionId")]
        public string OnePayTransactionId { get; set; }
        [Column("OnePayTransactionRequestDatetime")]
        public string OnePayTransactionRequestDatetime { get; set; }
        [Column("OnePayStatus")]
        public bool OnePayStatus { get; set; }
        [Column("OnePayPaidOn")]
        public string OnePayPaidOn { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        [ForeignKey("PackageId")]
        public virtual Packages Package { get; set; } = null!;
    }
}
