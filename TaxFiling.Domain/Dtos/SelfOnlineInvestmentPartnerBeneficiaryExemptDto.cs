

namespace TaxFiling.Domain.Dtos; 

public class SelfOnlineInvestmentPartnerBeneficiaryExemptDto
{
    public int InvestmentIncomePBEId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Category { get; set; }
    public string TransactionType { get; set; }
    public string TotalInvestmentIncome { get; set; }
    public string ActivityCode { get; set; }
    public string PartnershipName { get; set; }
    public string TrustName { get; set; }
    public string TINNO { get; set; }
    public decimal? GainsProfits { get; set; }
    public decimal? TotalInvestmentIncomePartnership { get; set; }
    public decimal? TotalInvestmentIncomeTrust { get; set; }
    public bool? IsExemptAmountA { get; set; }
    public bool? IsExcludedAmountB { get; set; }
    public decimal? ExemptExcludedIncome { get; set; }
}
