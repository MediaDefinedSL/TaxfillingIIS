using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;

namespace TaxFiling.Business.Interfaces;

public interface IPackagesRepository
{

    int Result { get; set; }
    Guid ResultGuid { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
    Task<List<PackagesDto>> GetPackages(int IsSelfFiling,  CancellationToken cancellationToken);
    Task<PackagesDto?> GetPackageDetails(int id, CancellationToken ctx);

}

