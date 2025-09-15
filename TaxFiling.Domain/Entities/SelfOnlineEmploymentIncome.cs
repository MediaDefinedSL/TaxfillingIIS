using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("SelfOnline_EmploymentIncome")]
public class SelfOnlineEmploymentIncome : Entity
{
    [Column("SelfOnlineEmploymentIncomeId")]
    [Key]
    public int SelfOnlineEmploymentIncomeId { get; set; }
    [Column("UserId")]
    public string UserId { get; set; }
    [Column("Year")]
    public int Year { get; set; }
    [Column("Residency")]
    public int? Residency { get; set; }
    [Column("SeniorCitizen")]
    public bool SeniorCitizen { get; set; }
    [Column("TerminalBenefits")]
    public bool? TerminalBenefits { get; set; }
    [Column("ExemptAmounts")]
    public bool? ExemptAmounts { get; set; }
    [Column("Total")]
    public decimal? Total { get; set; }

    public decimal? BenefitExcludedForTax { get; set; }


}
