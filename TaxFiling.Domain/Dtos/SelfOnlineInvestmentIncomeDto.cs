using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class SelfOnlineInvestmentIncomeDto
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


    public string PBTotalInvestmentIncome { get; set; }
    public string ActivityCode { get; set; }
    public string PartnershipName { get; set; }
    public string TrustTIN { get; set; }
    public string PBGainsProfits { get; set; }
    public string TotalInvestmentIncomePartnership { get; set; }
    public string TotalInvestmentIncomeTrust { get; set; }

    public string IsExemptAmountA { get; set; }
    public string IsExcludedAmountB { get; set; }
    public string ExemptExcludedIncome { get; set; }
}
