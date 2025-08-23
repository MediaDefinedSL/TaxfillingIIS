using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxFiling.Domain.Dtos;

public class SelfFilingTotalCalculationDto
{
    public int SelfOnlineTotalId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public decimal? EmploymentIncomeTotal { get; set; }
    public decimal? EmpIncome_EmpDetails { get; set; }
    public decimal? EmpIncome_TermBenefits { get; set; }
    public decimal? EmpIncome_ExeAmounts { get; set; }
    public decimal? TaxTotal { get; set; }
}
