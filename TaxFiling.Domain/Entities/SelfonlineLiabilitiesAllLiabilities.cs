using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("selfonlineliabilities_allliabilitiess")]
public class SelfonlineLiabilitiesAllLiabilities
{
    [Key]
    [Column("SelfonlineLiabilityID")]
    public int SelfonlineLiabilityID { get; set; }

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

    [Column("SecurityOnLiability")]
    public string SecurityOnLiability { get; set; }
    [Column("DateOfCommencement")]
    public DateTime? DateOfCommencement { get; set; }
    [Column("OriginalAmount")]
    public decimal? OriginalAmount { get; set; }
    [Column("AmountAsAt")]
    public decimal? AmountAsAt { get; set; }
    [Column("AmountRepaid")]
    public decimal? AmountRepaid { get; set; }
}
