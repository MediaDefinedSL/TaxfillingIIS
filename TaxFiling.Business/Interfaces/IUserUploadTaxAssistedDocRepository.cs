using Microsoft.AspNetCore.Mvc;
using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces;

public interface IUserUploadTaxAssistedDocRepository
{
    Task<List<UserUploadTaxAssistedDocDto>> GetUploadUserList(CancellationToken cancellationToken);
    Task<int?> SaveUploadedDocsContent(UserUploadTaxAssistedDocDto input);

    Task<List<UserUploadTaxAssistedDocDto>> GetUploadedDocsByUser(string userId);

    Task<bool> DeleteUploadedDocAsync(int userUploadId);
    Task<int?> SubmitAssetsAsync(UserUploadTaxAssistedDocDto asset);
    Task<bool> DeleteAllUploadedDocsByUserAndYear(string userId, int assessmentYear);
}
