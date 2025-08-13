using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class SelfOnlineEmploymentIncomeDetailsDto
{
    public int SelfOnlineEmploymentDetailsId { get; set; }
    public int SelfOnlineEmploymentIncomeId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string CategoryName { get; set; }
    public string TypeOfName { get; set; }
    public string EmployerORCompanyName { get; set; }
    public string TINOfEmployer { get; set; }
    public decimal? Remuneration { get; set; }
    public decimal? APITPrimaryEmployment { get; set; }
    public decimal? APITSecondaryEmployment { get; set; }
    public decimal? TerminalBenefits { get; set; }
    public decimal? Amount { get; set; }
   
}
