using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces;

public interface ISelfOnlineFlowRepository
{
    Task<List<TaxPayerDetailsDto>> GetTaxPayers(string userId, int year, CancellationToken ctx);
    Task<List<MaritalStatusDetailsDto>> GetMaritalStatus(string userId, int year, CancellationToken ctx);
    Task<List<TaxReturnLastyearDto>> GetTaxReturnLastyears(CancellationToken cancellationToken);
    Task<bool> SaveUserIdYear(string userId, int year);
    Task<SelfOnlineFlowPersonalInformationDto> GetSelfOnlineFlowPersonalInformationDetails(string userId, int year, CancellationToken ctx);
    Task<bool> UpdateTaxPayer(TaxPayerDetailsDto taxPayerdetails);
    Task<bool> UpdateMaritalStatus(MaritalStatusDetailsDto maritalStatusDetails);
    Task<bool> UpdatelLastYear(string userId, int year, int lastyearId);
    Task<bool> UpdatelIdentification(IdentificationsDto identifications);
    Task<bool> UpdatelContactInformation(ContactInfromationDto contactInfromation);

    Task<SelfFilingTotalCalculationDto?> GetSelfFilingTotalCalculation(string userId, int year, CancellationToken ctx);
    Task<bool> SaveSelfOnlineEmploymentIncome(SelfOnlineEmploymentIncomeDto selfOnlineEmploymentIncome);
    Task<SelfOnlineEmploymentIncomeDto> GetSelfOnlineEmploymentIncome(string userId, int year, CancellationToken ctx);
    Task<bool> SaveSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails);
    Task<List<SelfOnlineEmploymentIncomeDetailsDto>> GetSelfOnlineEmploymentIncomeList(string userId, int year, CancellationToken ctx);
    Task<bool> UpdateEmploymentIncomeTerminalBenefits(string userId, int year, int employmentIncomeId, bool terminalBenefits);

    Task<bool> UpdateEmploymentIncomeExemptAmounts(string userId, int year, int employmentIncomeId, bool exemptAmounts);
    Task<bool> UpdateSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails);
    Task<bool> DeleteEmploymentIncomeDetail(string userId, int year, int employmentDetailsId, string employmentDetailsName);


    //----------InvestmentIncome
    Task<bool> SaveSelfOnlineInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDto selfOnlineInvestmentIncom);
    Task<List<SelfOnlineInvestmentIncomeDto>> GetSelfOnlineInvestmentIncomeList(string userId, int year, CancellationToken ctx);
    Task<bool> DeleteInvestmentIncomeDetail(string userId, int year, int investmentIncomeId, string categoryName);

    //----------InvestmentIncome new 
    Task<bool> SaveSelfOnlineInvestmentDetails(SelfOnlineInvestmentIncomeDetailDto selfOnlineInvestmentIncomeDetail);
    Task<List<SelfOnlineInvestmentIncomeDetailDto>> GetSelfOnlineInvestmentIncomeDetailsList(string userId, int year, CancellationToken ctx);
    Task<bool> DeleteSelfOnlineInvestmentDetails(string userId, int year, int investmentIncomeId, string categoryName);
    Task<bool> SaveSelfOnlineInvestmentPartnerBeneficiaryExempt(SelfOnlineInvestmentPartnerBeneficiaryExemptDto selfOnlineInvestmentIncomeDetail);
    Task<List<SelfOnlineInvestmentPartnerBeneficiaryExemptDto>> GetSelfOnlineInvestmentPartnerBeneficiaryExempt(string userId, int year, CancellationToken ctx);
    Task<bool> DeleteSelfOnlineInvestmentPartnerBeneficiaryExempt(string userId, int year, int investmentIncomeId, string categoryName);

    Task<bool> UpdateSelfFilingTotalCalculation(SelfFilingTotalCalculationDto totalCalculation);

    //-------- Assets and Liabilities
    //-------- Assets
    Task<bool> SaveSelfonlineAssetsImmovableProperty(SelfonlineAssetsImmovablePropertyDto immovableProperties);
    Task<bool> SaveSelfonlineAssetsMotorVehicle(SelfonlineAssetsMotorVehicleDto motorVehicles);
}
