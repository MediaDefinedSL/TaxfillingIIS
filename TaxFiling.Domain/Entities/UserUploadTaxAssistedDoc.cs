using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("userupload_taxassisted_doc")]
public class UserUploadTaxAssistedDoc
{
    [Column("UserUploadId")]
    [Key]
    public int UserUploadId { get; set; }
    [Column("UserId")]
    public string UserId { get; set; }
    [Column("year")]
    public int Year { get; set; }
    [Column("UploadDate")]
    public DateTime? UploadDate { get; set; }
    [Column("categoryName")]
    public string CategoryName { get; set; }
    [Column("T10EmployerName")]
    public string T10EmployerName { get; set; }
    [Column("TerminalEmployerName")]
    public string TerminalEmployerName { get; set; }
    [Column("AnyOtherType")]
    public string AnyOtherType { get; set; }
    [Column("BankConfirmationType")]
    public string BankConfirmationType { get; set; }
    [Column("BankName")]
    public string BankName { get; set; }
    [Column("uploadedFileName")]
    public string UploadedFileName { get; set; }
    [Column("fileName")]
    public string FileName { get; set; }
    [Column("location")]
    public string Location { get; set; }
    [Column("decryptionKey")]
    public string DecryptionKey { get; set; }
    [Column("uploadId")]
    public string UploadId { get; set; }
    [Column("originalName")]
    public string OriginalName { get; set; }
    [Column("uploadTime")]
    public DateTime? UploadTime { get; set; }
    [Column("OtherDocumentName")]
    public string OtherDocumentName { get; set; }
    [Column("AssestOptionType")]
    public int AssestOptionType { get; set; }
    [Column("AssestsUploadExcelSheetName")]
    public string AssestsUploadExcelSheetName { get; set; }
    [Column("AssestType")]
    public int? AssestType { get; set; }
    [Column("AssetCategory")]    
    public string AssetCategory { get; set; }    
    [Column("AssetNote")]
    public string AssetNote { get; set; }
    [Column("AssetVehicleType")]
    public string AssetVehicleType { get; set; }
    [Column("AssetInstitution")]
    public string AssetInstitution { get; set; }
    


}

