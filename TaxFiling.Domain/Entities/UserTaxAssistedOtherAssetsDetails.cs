using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Entities
{
    [Table("userTaxAssistedOtherAssetsDetails")]
   public class UserTaxAssistedOtherAssetsDetails
    {
        [Column("OtherAssetEntryId")]
        [Key]
        public int OtherAssetEntryId { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        [Column("OtherAssetCategory")]
        public string OtherAssetCategory { get; set; }
        [Column("OtherAssetValue")]
        public decimal OtherAssetValue { get; set; }
        [Column("AssessmentYear")]
        public string AssessmentYear { get; set; }
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [Column("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }

    }
}
