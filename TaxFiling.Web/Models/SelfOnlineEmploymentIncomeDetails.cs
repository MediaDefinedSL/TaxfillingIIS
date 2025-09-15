namespace TaxFiling.Web.Models;

public class SelfOnlineEmploymentIncomeDetails
{
    public int SelfOnlineEmploymentDetailsId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string CategoryName { get; set; }
    public int? Residency { get; set; }
    public bool SeniorCitizen { get; set; }
    public string TypeOfName { get; set; }
    public string EmployerORCompanyName { get; set; }
    public string TINOfEmployer { get; set; }
    public decimal? Remuneration { get; set; }
    public decimal? APITPrimaryEmployment { get; set; }
    public decimal? APITSecondaryEmployment { get; set; }
    public decimal? TerminalBenefits { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Total { get; set; }
    public decimal? BenefitExcludedForTax { get; set; }
    public string UploadedFileName { get; set; }
    // External API values
    public string FileName { get; set; }
    public string Location { get; set; }
    public string DecryptionKey { get; set; }
    public string UploadId { get; set; }
    public string OriginalName { get; set; }
    public DateTime? UploadTime { get; set; }
}
