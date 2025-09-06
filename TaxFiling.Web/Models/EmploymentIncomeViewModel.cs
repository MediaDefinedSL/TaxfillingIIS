namespace TaxFiling.Web.Models;

public class EmploymentIncomeViewModel
{
    // Part I
    public bool IsResident { get; set; }
    public bool IsSeniorCitizen { get; set; }
    public string EmploymentType { get; set; }
    public string EmployerName { get; set; }
    public string EmployerTIN { get; set; }
    public decimal Remuneration { get; set; }
    public decimal APITPrimary { get; set; }
    public decimal APITSecondary { get; set; }

    // Terminal Benefits
    public string TerminalBenefitType { get; set; }
    public string TerminalEmployerName { get; set; }
    public string TerminalEmployerTIN { get; set; }
    public decimal TerminalBenefitAmount { get; set; }

    // Exempt Income
    public string ExemptType { get; set; }
    public string ExemptEmployerTIN { get; set; }
    public decimal ExemptAmount { get; set; }
    public decimal? Total { get; set; }
}
