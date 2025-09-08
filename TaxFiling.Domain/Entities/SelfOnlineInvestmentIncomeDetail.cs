using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("selfonlineinvestmentincomedetails")]
public class SelfOnlineInvestmentIncomeDetail : Entity
{
    [Key]
    [Column("InvestmentIncomeDetailId")]
    public int InvestmentIncomeDetailId { get; set; }
    [Column("UserId")]
    public string UserId { get; set; }
    [Column("Year")]
    public int Year { get; set; }
    [Column("Category")]
    public string Category { get; set; }
    [Column("ActivityCode")]
    public string ActivityCode { get; set; }
    [Column("TypeOfInvestment")]
    public string TypeOfInvestment { get; set; }
    [Column("AmountInvested")]
    public decimal? AmountInvested { get; set; }
    [Column("IncomeAmount")]
    public decimal? IncomeAmount { get; set; }
    [Column("WHTDeducted")]
    public decimal? WHTDeducted { get; set; }
    [Column("ForeignTaxCredit")]
    public decimal? ForeignTaxCredit { get; set; }
    [Column("BankName")]
    public string BankName { get; set; }
    [Column("BankBranch")]
    public string BankBranch { get; set; }
    [Column("AccountNo")]
    public string AccountNo { get; set; }
    [Column("WHTCertificateNo")]
    public string WHTCertificateNo { get; set; }
    [Column("OpeningBalance")]
    public decimal? OpeningBalance { get; set; }
    [Column("ClosingBalance")]
    public decimal? ClosingBalance { get; set; }
    [Column("CompanyInstitution")]
    public string CompanyInstitution { get; set; }
    [Column("SharesStocks")]
    public string SharesStocks { get; set; }
    [Column("AcquisitionDate")]
    public DateTime? AcquisitionDate { get; set; }
    [Column("CostAcquisition")]
    public decimal? CostAcquisition { get; set; }
    [Column("NetDividendIncome")]
    public decimal? NetDividendIncome { get; set; }
    [Column("PropertySituation")]
    public string PropertySituation { get; set; }
    [Column("PropertyAddress")]
    public string PropertyAddress { get; set; }
    [Column("DeedNo")]
    public string DeedNo { get; set; }
    [Column("RatesLocalAuthority")]
    public decimal? RatesLocalAuthority { get; set; }
    [Column("GiftOrInheritedCost")]
    public decimal? GiftOrInheritedCost { get; set; }
    [Column("MarketValue")]
    public decimal? MarketValue { get; set; }
    [Column("InterestIncome")]
    public decimal? InterestIncome { get; set; }
}
