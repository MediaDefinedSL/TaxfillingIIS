namespace TaxFiling.Web.Models;

public class SelfonlineAssetsSharesStocksSecuritiesViewModel
{
    public int SelfonlineSharesStocksID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string TransactionType { get; set; }
    public string SerialNumber { get; set; }
    public string CompanyName { get; set; }
    public int? NoOfSharesStocks { get; set; }
    public DateTime? DateOfAcquisition { get; set; }
    public decimal? CostOfAcquisition { get; set; }
    public decimal? NetDividendIncome { get; set; }
}
