namespace TaxFiling.Web.Models;

public class TaxReportViewModel
{
    public string TaxpayerName { get; set; }
    public string TaxpayerId { get; set; }
    public int TaxYear { get; set; }
    public DateTime GeneratedOn { get; set; } = DateTime.Now;

    // Income section
    public decimal EmploymentIncome { get; set; }
    public decimal BusinessIncome { get; set; }
    public decimal InvestmentIncome { get; set; }
    public decimal OtherIncome { get; set; }

    // Deductions
    public decimal TotalDeductions { get; set; }

    // Tax calculation
    public decimal TaxableIncome => EmploymentIncome + BusinessIncome + InvestmentIncome + OtherIncome - TotalDeductions;
    public decimal TaxPayable { get; set; }

    // Optional: breakdown list
  //  public List<TaxReportItem> IncomeBreakdown { get; set; } = new List<TaxReportItem>();
}
