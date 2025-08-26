namespace TaxFiling.Web.Models;

public class SelfOnlineInvestmentIncomeDetails
{
    public int SelfOnlineInvestmentId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Category { get; set; }
    public string InvestmentIncomeType { get; set; }

    public decimal? Remuneration { get; set; }
    public decimal? GainsProfits { get; set; }
    public decimal? TotalInvestmentIncome { get; set; }

    public string BankName { get; set; }
    public string BankBranch { get; set; }
    public string AccountNo { get; set; }
    public decimal? AmountInvested { get; set; }
    public decimal? Interest { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? Balance { get; set; }

    public string CompanyInstitution { get; set; }
    public int? SharesStocks { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public decimal? CostAcquisition { get; set; }
    public decimal? NetDividendIncome { get; set; }

    public string PropertyDeedNo { get; set; }
    public DateTime? RentAcquisitionDate { get; set; }
    public decimal? CostGiftInherited { get; set; }
    public decimal? MarketValue { get; set; }
}
