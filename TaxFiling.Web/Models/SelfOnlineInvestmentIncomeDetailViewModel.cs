namespace TaxFiling.Web.Models;

public class SelfOnlineInvestmentIncomeDetailViewModel
{
    public int InvestmentIncomeDetailId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string TransactionType { get; set; }
    public string Category { get; set; }   // Savings, FD, Dividend, Rent

    public string ActivityCode { get; set; }
    public string TypeOfInvestment { get; set; }

    public decimal? AmountInvested { get; set; }
    public decimal? IncomeAmount { get; set; }
    public decimal? WHTDeducted { get; set; }
    public decimal? ForeignTaxCredit { get; set; }

    // Savings & FD
    public string BankName { get; set; }
    public string BankBranch { get; set; }
    public string AccountNo { get; set; }
    public string WHTCertificateNo { get; set; }
    public decimal? OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }

    // Dividend
    public string CompanyInstitution { get; set; }
    public string SharesStocks { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public decimal? CostAcquisition { get; set; }
    public decimal? NetDividendIncome { get; set; }

    // Rent
    public string PropertySituation { get; set; }
    public string PropertyAddress { get; set; }
    public string DeedNo { get; set; }
    public decimal? RatesLocalAuthority { get; set; }
    public decimal? GiftOrInheritedCost { get; set; }
    public decimal? MarketValue { get; set; }
}
