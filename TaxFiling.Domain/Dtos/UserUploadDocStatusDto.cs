using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Dtos
{
    public class UserUploadDocStatusDto
    {
        /// <summary>
        /// The unique ID of the user.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The tax year this status applies to.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The document status code (for example: 0 = Pending, 1 = Completed, etc.).
        /// </summary>
        public int DocStatus { get; set; }

        
        public int? IsPersonalInfoCompleted { get; set; }
        public int? IsIncomeTaxCreditsCompleted { get; set; }

        /// <summary>
        /// Optional date the status was created or updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
