namespace TaxFiling.Web.Models;

public class SelfonlineAssetsImmovablePropertyViewModel
{
    public int SelfonlinePropertyID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string TransactionType { get; set; }
    public string Type { get; set; }   // Local / Foreign
    public string SerialNumber { get; set; }
    public string Situation { get; set; }
    public DateTime? DateOfAcquisition { get; set; }
    public decimal? Cost { get; set; }
    public decimal? MarketValue { get; set; }
}
