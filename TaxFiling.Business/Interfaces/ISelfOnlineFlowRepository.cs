using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces;

public interface ISelfOnlineFlowRepository
{
    Task<List<TaxPayerDto>> GetTaxPayers(CancellationToken cancellationToken);
    Task<List<MaritalStatusDto>> GetMaritalStatus(CancellationToken cancellationToken);
    Task<List<TaxReturnLastyearDto>> GetTaxReturnLastyears(CancellationToken cancellationToken);
    Task<bool> SaveUserIdYear(string userId, int year);
    Task<SelfOnlineFlowPersonalInformationDto> GetSelfOnlineFlowPersonalInformationDetails(string userId, int year, CancellationToken ctx);
    Task<bool> UpdateTaxPayer(string userId, int year, int taxPayerId);
    Task<bool> UpdateMaritalStatus(string userId, int year, int maritalStatusId);
    Task<bool> UpdatelLastYear(string userId, int year, int lastyearId);
    Task<bool> UpdatelIdentification(string userId, int year, string firstName, string middleName, string lastName, DateTime dateofbirth, string taxnumber);
    Task<bool> UpdatelContactInformation(string userId, int year, string careof, string apt, string streetnumber, string street, string city);
}
