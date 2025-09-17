namespace TaxFiling.Web.Models;

public class SelfFilingSummaryCalculationViewModel
{
    public string UserId { get; set; }

    public decimal EmploymentIncomeTotal { get; set; }
    public decimal BusinessIncomeTotal { get; set; }
    public decimal InvestmentIncomeTotal { get; set; }

    public decimal InterestIncome { get; set; }
    public decimal InvIncome_Dividend { get; set; }
    public decimal InvIncome_Rent { get; set; }
    public decimal InvIncome_Other { get; set; }

    public decimal OtherIncomeTotal { get; set; }
    public decimal AssessableIncome { get; set; }

    public decimal PersonalRelief { get; set; }
    public decimal RentIncomeRelief { get; set; }
    public decimal SolarRelief { get; set; }
    public decimal ForeignServiceRelief { get; set; }

    public decimal TotalRelief { get; set; }
    public decimal QualifyingPayments { get; set; }
    public decimal TotalDeductions { get; set; }

    public decimal TaxableIncome { get; set; }

    public decimal TaxOnTerminalBenefits { get; set; }
    public decimal TaxOnInvestmentGains { get; set; }

    public decimal TaxBracket1 { get; set; }
    public decimal TaxBracket2 { get; set; }
    public decimal TaxBracket3 { get; set; }
    public decimal TaxBracket4 { get; set; }
    public decimal TaxBracket5 { get; set; }
    public decimal TaxBracket6 { get; set; }

    public decimal TotalTaxPayable { get; set; }
    public decimal TaxOnBalanceIncome { get; set; }

    public decimal TaxCredits { get; set; }
    public decimal BalanceTaxPayable { get; set; }

    public decimal CashInHandStart { get; set; }
    public decimal CashAtBankStart { get; set; }

    public decimal EPFContribution { get; set; }
    public decimal TaxPayments { get; set; }
    public decimal LivingEducationExpenses { get; set; }
    public decimal ChildrenEducationExpenses { get; set; }
    public decimal VehicleExpenses { get; set; }
    public decimal LifeMedicalInsurance { get; set; }
    public decimal LoanInstallments { get; set; }
    public decimal CreditCardExpenses { get; set; }
    public decimal PurchaseEquitySecurities { get; set; }
    public decimal AnyOtherExpenses { get; set; }
    public decimal TotalCashOutflow { get; set; }

    public decimal CashInHandEnd { get; set; }
    public decimal CashAtBankEnd { get; set; }
}
