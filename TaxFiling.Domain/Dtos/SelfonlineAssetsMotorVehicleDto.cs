

namespace TaxFiling.Domain.Dtos;

public class SelfonlineAssetsMotorVehicleDto
{
    public int SelfonlineMotorVehicleID { get; set; }
    public int UserId { get; set; }
    public int Year { get; set; }
    public string TransactionType { get; set; }
    public string Type { get; set; }   // Local / Foreign
    public string SerialNumber { get; set; }
    public string Description { get; set; }
    public string RegistrationNo { get; set; }
    public DateTime? DateOfAcquisition { get; set; }
    public decimal? CostMarketValue { get; set; }
}
