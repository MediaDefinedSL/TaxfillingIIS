using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("SelfFilingTotalCalculation")]
public class SelfFilingTotalCalculation
{
    [Column("SelfOnlineTotalId")]
    [Key]
    public int SelfOnlineTotalId { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }
    [Column("Year")]
    public int Year { get; set; }

    [Column("EmploymentIncomeTotal")]
    public decimal? EmploymentIncomeTotal { get; set; }

    [Column("EmpIncome_EmpDetails")]
    public decimal? EmpIncome_EmpDetails { get; set; }

    [Column("EmpIncome_TermBenefits")]
    public decimal? EmpIncome_TermBenefits { get; set; }

    [Column("EmpIncome_ExeAmounts")]
    public decimal? EmpIncome_ExeAmounts { get; set; }
    [Column("InvestmentIncomeTotal")]
    public decimal? InvestmentIncomeTotal { get; set; }
    [Column("InvIncome_Savings")]
    public decimal? InvIncome_Savings { get; set; }
    [Column("InvIncome_FixedDeposit")]
    public decimal? InvIncome_FixedDeposit { get; set; }
    [Column("InvIncome_Dividend")]
    public decimal? InvIncome_Dividend { get; set; }
    [Column("InvIncome_Rent")]
    public decimal? InvIncome_Rent { get; set; }
    [Column("InvIncome_Partner")]
    public decimal? InvIncome_Partner { get; set; }
    [Column("InvIncome_Beneficiary")]
    public decimal? InvIncome_Beneficiary { get; set; }
    [Column("InvIncome_ExemptAmounts")]
    public decimal? InvIncome_ExemptAmounts { get; set; }
    [Column("InvIncome_Other")]
    public decimal? InvIncome_Other { get; set; }
    [Column("ReliefSolarPanel")]
    public decimal? ReliefSolarPanel { get; set; }
    [Column("QualifyingPayments")]
    public decimal? QualifyingPayments { get; set; }

    [Column("TaxTotal")]
    public decimal? TaxTotal { get; set; }
}
