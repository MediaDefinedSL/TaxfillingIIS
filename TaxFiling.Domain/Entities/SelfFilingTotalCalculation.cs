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

    [Column("TaxTotal")]
    public decimal? TaxTotal { get; set; }
}
