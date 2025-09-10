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
    public decimal? InvestmentIncomeTotal { get; set; }
    public decimal? InvIncome_Savings { get; set; }
    public decimal? InvIncome_FixedDeposit { get; set; }
    public decimal? InvIncome_Dividend { get; set; }
    public decimal? InvIncome_Rent { get; set; }
    public decimal? InvIncome_Partner { get; set; }
    public decimal? InvIncome_Beneficiary { get; set; }
    public decimal? InvIncome_ExemptAmounts { get; set; }
    public decimal?  InvIncome_Other { get; set; }
    public decimal? ReliefSolarPanel { get; set; }
    public decimal? QualifyingPayments { get; set; }
    public decimal? TaxTotal { get; set; }
}
