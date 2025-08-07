using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Dtos
{
    public class UserUploadedDocumentStatusDto
    {
        public Guid UserId { get; set; }
        public int? UploadedDocumentStatus { get; set; }
    }
}
