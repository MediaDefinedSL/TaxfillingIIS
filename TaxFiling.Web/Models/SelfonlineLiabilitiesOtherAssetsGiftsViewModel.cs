namespace TaxFiling.Web.Models;

public class SelfonlineLiabilitiesOtherAssetsGiftsViewModel
{
    public int SelfonlineAssetsGiftsID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string TransactionType { get; set; }
    public string SerialNumber { get; set; }
    public string Description { get; set; }
    public string AcquisitionMode { get; set; }
    public DateTime? DateOfAcquisition { get; set; }
    public decimal? CostMarketValue { get; set; }
}
