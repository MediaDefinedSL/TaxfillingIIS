using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("selfonlineliabilities_disposalassets")]
public class SelfonlineLiabilitiesDisposalAssets
{
    [Key]
    [Column("SelfonlineDisposalAssetsID")]
    public int SelfonlineDisposalAssetsID { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }

    [Column("Year")]
    public int Year { get; set; }

    [Column("Type")]
    public string Type { get; set; }

    [Column("SerialNumber")]
    public string SerialNumber { get; set; }

    [Column("Description")]
    public string Description { get; set; }
    [Column("DateOfDisposal")]
    public DateTime? DateOfDisposal { get; set; }
    [Column("SalesProceed")]
    public decimal? SalesProceed { get; set; }
    [Column("DateAcquired")]
    public DateTime? DateAcquired { get; set; }
    [Column("Cost")]
    public decimal? Cost { get; set; }
}
