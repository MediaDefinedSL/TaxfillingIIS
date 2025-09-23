using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaxFiling.Domain.Entities
{
    [Table("userUploadDocStatus")]
    public class UserUploadDocStatus
    {
        [Column("UserUploadDocId")]
        [Key]
        public int UserUploadDocId { get; set; }           // Primary key (identity)

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("docStatus")]
        public int DocStatus { get; set; }

        [Column("isPersonalInfoCompleted")]
        public int IsPersonalInfoCompleted { get; set; }

        [Column("isIncomeTaxCreditsCompleted")]
        public int IsIncomeTaxCreditsCompleted { get; set; }
        [Column("UpdatedDate")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
