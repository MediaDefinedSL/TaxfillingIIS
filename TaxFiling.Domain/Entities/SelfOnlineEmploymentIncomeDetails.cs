using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("SelfOnlineEmploymentIncome")]
public class SelfOnlineEmploymentIncomeDetails : Entity
{
    [Column("SelfOnlineEmploymentDetailsId")]
    [Key]
    public int SelfOnlineEmploymentDetailsId { get; set; }
    
    [Column("UserId")]
    public string UserId { get; set; }
    [Column("Year")]
    public int Year { get; set; }
    [Column("CategoryName")]
    public string CategoryName { get; set; }
    [Column("Residency")]
    public int? Residency { get; set; }
    [Column("SeniorCitizen")]
    public bool SeniorCitizen { get; set; }
    [Column("TypeOfName")]
    public string TypeOfName { get; set; }
    [Column("EmployerORCompanyName")]
    public string EmployerORCompanyName { get; set; }
    [Column("TINOfEmployer")]
    public string TINOfEmployer { get; set; }
    [Column("Remuneration")]
    public decimal? Remuneration { get; set; }
    [Column("APITPrimaryEmployment")]
    public decimal? APITPrimaryEmployment { get; set; }
    [Column("APITSecondaryEmployment")]
    public decimal? APITSecondaryEmployment { get; set; }
    [Column("TerminalBenefits")]
    public decimal? TerminalBenefits { get; set; }
    [Column("Amount")]
    public decimal? Amount { get; set; }
    [Column("BenefitExcludedForTax")]
    public decimal? BenefitExcludedForTax { get; set; }

}
