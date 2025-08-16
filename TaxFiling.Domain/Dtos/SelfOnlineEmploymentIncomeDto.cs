using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class SelfOnlineEmploymentIncomeDto
{
   public int SelfOnlineEmploymentIncomeId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public int? Residency { get; set; }
    public bool SeniorCitizen { get; set; }
    public bool? TerminalBenefits { get; set; }
    public bool? ExemptAmounts { get; set; }
    public decimal? Total { get; set; }

}
