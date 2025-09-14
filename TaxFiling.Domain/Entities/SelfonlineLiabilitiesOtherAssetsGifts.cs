using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("selfonlineliabilities_otherassetsgifts")]
public class SelfonlineLiabilitiesOtherAssetsGifts
{
    [Key]
    [Column("SelfonlineAssetsGiftsID")]
    public int SelfonlineAssetsGiftsID { get; set; }

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

    [Column("AcquisitionMode")]
    public string AcquisitionMode { get; set; }
    [Column("DateOfAcquisition")]
    public DateTime? DateOfAcquisition { get; set; }
    [Column("CostMarketValue")]
    public decimal? CostMarketValue { get; set; }
}
