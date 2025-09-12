using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TaxFiling.Domain.Entities;
[Table("selfonlineinvestmentpartnerbeneficiaryexempt")]
public class SelfOnlineInvestmentPartnerBeneficiaryExempt
{
    [Key]
    [Column("InvestmentIncomePBEId")]
    public int InvestmentIncomePBEId { get; set; }
    [Column("UserId")]
    public string UserId { get; set; }
    [Column("Year")]
    public int Year { get; set; }
    [Column("Category")]
    public string Category { get; set; }
    [Column("TotalInvestmentIncome")]
    public string TotalInvestmentIncome { get; set; }
    [Column("ActivityCode")]
    public string ActivityCode { get; set; }
    [Column("PartnershipName")]
    public string PartnershipName { get; set; }
    [Column("TrustName")]
    public string TrustName { get; set; }
    [Column("TINNO")]
    public string TINNO { get; set; }

    [Column("GainsProfits")]
    public decimal? GainsProfits { get; set; }

    [Column("TotalInvestmentIncomePartnership")]
    public decimal? TotalInvestmentIncomePartnership { get; set; }

    [Column("TotalInvestmentIncomeTrust")]
    public decimal? TotalInvestmentIncomeTrust { get; set; }

    [Column("IsExemptAmountA")]
    public bool? IsExemptAmountA { get; set; }

    [Column("IsExcludedAmountB")]
    public bool? IsExcludedAmountB { get; set; }

    [Column("ExemptExcludedIncome")]
    public decimal? ExemptExcludedIncome { get; set; }
    [Column("ExemptExcludedIncomeName")]
    public string ExemptExcludedIncomeName { get; set; }
}
