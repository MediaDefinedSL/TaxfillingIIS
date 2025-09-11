using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TaxFiling.Domain.Entities;
[Table("selfonlineAssets_motorVehicles")]
public class SelfonlineAssetsMotorVehicle
    {
    [Key]
    [Column("SelfonlinePropertyID")]
    public int SelfonlineMotorVehicleID { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }  

    [Column("Year")]
    public int Year { get; set; }

    [Column("Type")]
    public string Type { get; set; }  

    [Column("SerialNumber")]
    public string SerialNumber { get; set; }
    [Column("Description")]
    public string Description { get; set; }  

    [Column("RegistrationNo")]
    public string RegistrationNo { get; set; }
    [Column("DateOfAcquisition")]
    public DateTime? DateOfAcquisition { get; set; }

    [Column("CostMarketValue")]
    public decimal? CostMarketValue { get; set; }

}

