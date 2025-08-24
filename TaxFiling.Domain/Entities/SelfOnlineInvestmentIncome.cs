using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("SelfOnlineInvestmentIncome")]
public class SelfOnlineInvestmentIncome : Entity
{
    [Key]
    [Column("SelfOnlineInvestmentId")]
    public int SelfOnlineInvestmentId { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }

    [Column("year")]
    public int Year { get; set; }

    [Column("Category")]
    public string Category { get; set; }   // Savings, FD, Dividend, Rent, Other Income

    [Column("InvestmentIncomeType")]
    public string InvestmentIncomeType { get; set; }  // Interest, Rent, Dividend, etc.

    [Column("Remuneration")]
    public decimal? Remuneration { get; set; }

    [Column("GainsProfits")]
    public decimal? GainsProfits { get; set; }

    [Column("TotalInvestmentIncome")]
    public decimal? TotalInvestmentIncome { get; set; }

    [Column("BankOrCompany")]
    [StringLength(100)]
    public string BankOrCompany { get; set; }

    [Column("AccountNo")]
    [StringLength(50)]
    public string AccountNo { get; set; }

    [Column("AmountInvested")]
    public decimal? AmountInvested { get; set; }

    [Column("Interest")]
    public decimal? Interest { get; set; }

    [Column("OpeningBalance")]
    public decimal? OpeningBalance { get; set; }

    [Column("Balance")]
    public decimal? Balance { get; set; }

    [Column("CompanyInstitution")]
    [StringLength(100)]
    public string CompanyInstitution { get; set; }

    [Column("SharesStocks")]
    public int? SharesStocks { get; set; }

    [Column("AcquisitionDate")]
    public DateTime? AcquisitionDate { get; set; }

    [Column("CostAcquisition")]
    public decimal? CostAcquisition { get; set; }

    [Column("NetDividendIncome")]
    public decimal? NetDividendIncome { get; set; }

    [Column("PropertyDeedNo")]
    public string PropertyDeedNo { get; set; }

    [Column("RentAcquisitionDate")]
    public DateTime? RentAcquisitionDate { get; set; }

    [Column("CostGiftInherited")]
    public decimal? CostGiftInherited { get; set; }

    [Column("MarketValue")]
    public decimal? MarketValue { get; set; }

   
}
