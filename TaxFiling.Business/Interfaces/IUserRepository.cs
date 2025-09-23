using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;

namespace TaxFiling.Business.Interfaces;

public interface IUserRepository
{

    int Result { get; set; }
    Guid ResultGuid { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
   string Name { get; set; }
   public object? Data { get;  set; }
    Task<List<UserDto>> GetUsers(CancellationToken cancellationToken);
    Task<UserDto?> GetUser(UserDto mUser);
    Task<bool> AddUser(UserDto user);
    Task<AccessTokenData?> ValidateUser(LoginModel loginModel);
    Task<string> GenerateRefreshToken(Guid userId);
    Task<(bool, AccessTokenData?)> IsValidRefreshToken(Guid userid, string refreshToken);
    Task<UserDto?> GetUser(Guid id, CancellationToken ctx);
    Task<bool> UpdateUser(UserDto user);

    Task<bool> UpdateUserTinStatus(Guid userId, int tinStatus);
    Task<bool> updateProfileImage(string userId, string profileImagePath);
    Task<bool> UpdateUserUploadedDocumentStatus(Guid userId, int? userUploadedDocStatus);
    Task<int?> GetLatestUploadedDocumentStatusAsync(Guid userId);

    Task<bool> UpdatePasswordAsync(string email, string newPassword);
    Task<int?> GetPersonalInformationCompleted(Guid userId);
    Task<bool> UpdateUserUploadedDocStatus(string userId,
            int year,
            int? userUploadedDocStatus,
            int? isPersonalInfoCompleted,
            int? isIncomeTaxCreditsCompleted);
}

