using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TaxFiling.Domain.Entities;
[Table("selfonlineAssets_immovableProperties")]

public class SelfonlineAssetsImmovableProperty
    {
    [Key]
    [Column("SelfonlinePropertyID")]
    public int SelfonlinePropertyID { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }

    [Column("Year")]
    public int Year { get; set; }

    [Column("Type")]
    public string Type { get; set; }   // Local / Foreign

    [Column("SerialNumber")]
    public string SerialNumber { get; set; }

    [Column("Situation")]
    public string Situation { get; set; }
    [Column("DateOfAcquisition")]
    public DateTime? DateOfAcquisition { get; set; }
    [Column("Cost")]
    public decimal? Cost { get; set; }

    [Column("MarketValue")]
    public decimal? MarketValue { get; set; }
}

