using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Dtos
{
    public class AssetUploadItem
    {
        public string AssetNote { get; set; }
        public IFormFile File { get; set; }
        public string AssetVehicleType { get; set; }
        public string AssetInstitution { get; set; }
    }
}
