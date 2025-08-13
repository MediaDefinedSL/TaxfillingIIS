using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class UserUploadTaxAssistedDocDto
{
    public int UserUploadId { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ProfileImagePath { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Year { get; set; }
    public DateTime? UploadDate { get; set; }
    public string CategoryName { get; set; }
    public string T10EmployerName { get; set; }
    public string TerminalEmployerName { get; set; }
    public string AnyOtherType { get; set; }
    public string BankConfirmationType { get; set; }
    public string BankName { get; set; }
    public string UploadedFileName { get; set; }
    // External API values
    public string FileName { get; set; }
    public string Location { get; set; }
    public string DecryptionKey { get; set; }
    public string UploadId { get; set; }
    public string OriginalName { get; set; }
    public DateTime? UploadTime { get; set; }
    public string OtherDocumentName { get; set; }
    public int AssestOptionType { get; set; }
    public string AssestsUploadExcelSheetName { get; set; }
    public int? AssestType { get; set; }
    public string AssetCategory { get; set; }
    public string AssetNote { get; set; }
    public List<string> AssetNotes { get; set; }
    public string AssetVehicleType { get; set; }
    public string AssetInstitution { get; set; }
    public List<IFormFile> Files { get; set; }

    public List<AssetUploadItem> Items { get; set; }
    public List<string> AssetVehicleTypes { get; set; } // Changed to List

    public List<string> AssetInstitutions { get; set; } // Changed to List



}
