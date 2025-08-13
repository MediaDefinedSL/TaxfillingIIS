using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Dtos
{
   public  class UserTaxAssistedOtherAssetsDetailsDto
    {
        public int OtherAssetEntryId { get; set; }
        public string UserId { get; set; }  // Use Guid type for user IDs if they are GUIDs
        public string OtherAssetCategory { get; set; }
        public decimal OtherAssetValue { get; set; }
        public string AssessmentYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<OtherTaxDetailDto> Details { get; set; }
    }
}
