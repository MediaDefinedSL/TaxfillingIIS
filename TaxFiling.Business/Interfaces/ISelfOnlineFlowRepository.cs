using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces;

public interface ISelfOnlineFlowRepository
{
    Task<List<TaxPayerDetailsDto>> GetTaxPayers(string userId, int year, CancellationToken ctx);
    Task<List<MaritalStatusDto>> GetMaritalStatus(CancellationToken cancellationToken);
    Task<List<TaxReturnLastyearDto>> GetTaxReturnLastyears(CancellationToken cancellationToken);
    Task<bool> SaveUserIdYear(string userId, int year);
    Task<SelfOnlineFlowPersonalInformationDto> GetSelfOnlineFlowPersonalInformationDetails(string userId, int year, CancellationToken ctx);
    Task<bool> UpdateTaxPayer(TaxPayerDetailsDto taxPayerdetails);
    Task<bool> UpdateMaritalStatus(string userId, int year, int maritalStatusId);
    Task<bool> UpdatelLastYear(string userId, int year, int lastyearId);
    Task<bool> UpdatelIdentification(IdentificationsDto identifications);
    Task<bool> UpdatelContactInformation(ContactInfromationDto contactInfromation);
    Task<bool> SaveSelfOnlineEmploymentIncome(SelfOnlineEmploymentIncomeDto selfOnlineEmploymentIncome);
    Task<SelfOnlineEmploymentIncomeDto> GetSelfOnlineEmploymentIncome(string userId, int year, CancellationToken ctx);
    Task<bool> SaveSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails);
    Task<List<SelfOnlineEmploymentIncomeDetailsDto>> GetSelfOnlineEmploymentIncomeList(string userId, int year, CancellationToken ctx);
    Task<bool> UpdateEmploymentIncomeTerminalBenefits(string userId, int year, int employmentIncomeId, bool terminalBenefits);

    Task<bool> UpdateEmploymentIncomeExemptAmounts(string userId, int year, int employmentIncomeId, bool exemptAmounts);
    Task<bool> UpdateSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails);
    Task<bool> DeleteEmploymentIncomeDetail(string userId, int year, int employmentDetailsId, string employmentDetailsName);

    Task<bool> SaveSelfOnlineInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDto selfOnlineInvestmentIncom);
}
