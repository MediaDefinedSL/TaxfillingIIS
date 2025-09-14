namespace TaxFiling.Web.Models;

public class SelfonlineLiabilitiesAllLiabilitiesViewModel
{
    public int SelfonlineLiabilityID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string TransactionType { get; set; }
    public string SerialNumber { get; set; }
    public string Description { get; set; }
    public string SecurityOnLiability { get; set; }
    public DateTime? DateOfCommencement { get; set; }
    public decimal? OriginalAmount { get; set; }
    public decimal? AmountAsAt { get; set; }
    public decimal? AmountRepaid { get; set; }
}
