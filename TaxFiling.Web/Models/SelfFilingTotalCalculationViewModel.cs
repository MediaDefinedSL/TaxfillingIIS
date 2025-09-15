namespace TaxFiling.Web.Models;

public class SelfFilingTotalCalculationViewModel
{
    public int SelfOnlineTotalId { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }

    // Employment Income
    public decimal? EmploymentIncomeTotal { get; set; }
    public decimal? EmpIncome_EmpDetails { get; set; }
    public decimal? EmpIncome_TermBenefits { get; set; }
    public decimal? EmpIncome_ExeAmounts { get; set; }

    // Investment Income
    public decimal? InvestmentIncomeTotal { get; set; }
    public decimal? InvIncome_Savings { get; set; }
    public decimal? InvIncome_FixedDeposit { get; set; }
    public decimal? InvIncome_Dividend { get; set; }
    public decimal? InvIncome_Rent { get; set; }
    public decimal? InvIncome_Partner { get; set; }
    public decimal? InvIncome_Beneficiary { get; set; }
    public decimal? InvIncome_ExemptAmounts { get; set; }
    public decimal? InvIncome_Other { get; set; }
    public decimal? ReliefSolarPanel {  get; set; }
   
    // Tax
    public decimal? TaxTotal { get; set; }

    // New Computed Property
    public decimal? InterestIncome
    {
        get
        {
            return (InvIncome_Savings ?? 0) + (InvIncome_FixedDeposit ?? 0);
        }
    }

    public decimal? InvestmentIncome
    {
        get
        {
            return (InterestIncome ?? 0) + (InvIncome_Dividend ?? 0  +(InvIncome_Rent ?? 0) + (InvIncome_Other ?? 0));
        }
    }
    public decimal? AssessableIncome
    {
        get
        {
            return (EmploymentIncomeTotal ?? 0) + (InvestmentIncome ?? 0);
        }
    }
    public decimal? RentRelief25
    {
        get
        {
            return (InvIncome_Rent ?? 0) * 0.25m;
        }
    }

    public decimal? PersonalRelief
    {
        get
        {
            var totalIncome = (EmploymentIncomeTotal ?? 0) + (InvestmentIncomeTotal ?? 0);

            if (totalIncome <= 1_800_000)
            {
                // If income is less than or equal to 1,800,000 → entire amount as relief
                return totalIncome;
            }
            else
            {
                // If income is greater → fixed 1,800,000
                return 1_800_000;
            }
        }
    }

    public decimal? QualifyingPayments { get; set; }
   

}

