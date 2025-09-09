using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.NetworkInformation;

namespace TaxFiling.Business.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Context _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly IConfiguration _configuration;

    public int Result { get; set; }
    public Guid ResultGuid { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public object? Data { get;  set; }

    public UserRepository(Context context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetUsers(CancellationToken cancellationToken)
    {
        List<UserDto> users = [];
        try
        {
            users = await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Password = u.Password
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return users;
    }

    public async Task<UserDto?> GetUser(UserDto mUser)
    {
        UserDto? user = null;

        try
        {
            user = await _context.Users
                              .AsNoTracking()
                              .Where(u => u.UserName == mUser.UserName
                                          && u.Password == mUser.Password)
                              .Select(u => new UserDto
                              {
                                  UserId = u.UserId,
                                  UserName = u.UserName,
                                  TaxTotal = u.TaxTotal
                              })
                              .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while loading user");
        }

        return user;
    }

    public async Task<AccessTokenData?> ValidateUser(LoginModel loginModel)
    {
        AccessTokenData? accessTokenData = null;

        try
        {
            var userEntity = await _context.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Email == loginModel.Username);

            if (userEntity is null)
            {
                return null;
            }

            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(new object(), userEntity.Password, loginModel.Password);

            if (result == PasswordVerificationResult.Success)
            {  
                

                accessTokenData = new AccessTokenData
                {
                    UserId = userEntity.UserId,
                    UserName = userEntity.UserName,
                    RoleId = userEntity.UserRoleId,                  
                    Number = userEntity.Phone ?? string.Empty,
                    FirstName = userEntity.FirstName ?? string.Empty,
                    LastName = userEntity.LastName ?? string.Empty,
                    Email = userEntity.Email ?? string.Empty,
                    IsTin = userEntity.IsTin,
                    IsActivePayment = userEntity.IsActivePayment,
                    NICNO = userEntity.NICNO ?? string.Empty,
                    TinNo = userEntity.TinNo ?? string.Empty,
                    PackageId = userEntity.PackageId,
                    ProfileImagePath = userEntity.ProfileImagePath,
                    UploadedDocumentStatus = userEntity.taxAssistedUserUploadDocsStatus

                };

                return accessTokenData;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while validate user");
        }

        return accessTokenData;
    }

    public async Task<string> GenerateRefreshToken(Guid userid)
    {
        string refreshToken = string.Empty;

        try
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            refreshToken = Convert.ToBase64String(randomNumber);

            var userRefreshToken = await _context.UserRefreshTokens
                                            .FirstOrDefaultAsync(u => u.UserId == userid);
            if (userRefreshToken != null)
            {
                userRefreshToken.RefreshToken = refreshToken;
            }
            else
            {
                await _context.UserRefreshTokens.AddAsync(new UserRefreshToken
                {
                    UserId = userid,
                    RefreshToken = refreshToken
                });
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while generating refresh token");
        }

        return refreshToken;
    }

    public async Task<(bool, AccessTokenData?)> IsValidRefreshToken(Guid userid, string refreshToken)
    {
        bool isValid = true;
        AccessTokenData? accessTokenData = null;

        var userRefreshToken = await _context.UserRefreshTokens
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(u => u.UserId == userid
                                                                    && u.RefreshToken == refreshToken);

        isValid = userRefreshToken != null;

        if (isValid)
        {
            accessTokenData = await _context.Users
                              .AsNoTracking()
                              .Where(u => u.UserId == userid)
                              .Select(u => new AccessTokenData
                              {
                                  UserId = u.UserId,
                                  UserName = u.UserName
                              })
                              .FirstOrDefaultAsync();

            if(accessTokenData is null)
            {
                return (isValid, accessTokenData);
            }

            var role = await _context.UserRoles
                            .Where(u => u.UserId == accessTokenData.UserId)
                            .Join(_context.Roles,
                                userRole => userRole.RoleId,
                                role => role.Code,
                                (userRole, role) => new
                                {
                                    role.Code,
                                    role.RoleName,
                                    role.Acronym
                                })
                            .FirstOrDefaultAsync();

            accessTokenData.RoleId = role.Code;
        }

        return (isValid, accessTokenData);
    }
    public async Task<bool> AddUser(UserDto User)
    {
        bool isSuccess = false;

        var dbTrans = await _context.Database.BeginTransactionAsync();
        try
        {

            // Check if email or phone already exists
            bool userEmailExists = await _context.Users
                .AnyAsync(u => u.Email == User.Email );

            bool userPhoneExists = await _context.Users
               .AnyAsync(u => u.Phone == User.Phone);

            if (userEmailExists)
            {
                Message = "A user with the same Email  already exists.";
                return false; // Do not continue
            }
            if (userPhoneExists)
            {
                Message = "A user with the same Phone number already exists.";
                return false; // Do not continue
            }

            var hasher = new PasswordHasher<object>();
            string hashedPassword = string.Empty;
            if (!string.IsNullOrWhiteSpace(User.Password))
            {
                hashedPassword = hasher.HashPassword(new object(), User.Password);
            }

            // generate user id
            var userId = Guid.NewGuid();

            var _user = new User
            {
                UserId = userId,
                UserName = User.Email,
                Password = hashedPassword,
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                UserRoleId = 3,
                Phone = User.Phone,
                IsActive = 1,
                TinNo=User.TinNo,
                NICNO = User.NICNO,
                CreatedBy = userId.ToString(),
                CreatedOn =DateTime.Now

            };

            _context.Users.Add(_user);
            await _context.SaveChangesAsync();


            await dbTrans.CommitAsync();

            isSuccess = true;
            Message = "Successfully registered the User.";
            ResultGuid = userId;
            Name = _user.FirstName + " " + _user.LastName;
            Data = new
            {
                _user.UserId,
                _user.FirstName,
                _user.LastName,
                _user.Email,
                _user.Phone,
                _user.TinNo,
                _user.NICNO
            };
        }
        catch (Exception e)
        {
            await dbTrans.RollbackAsync();
            Message = "An error occurred while registering the User.";
            _logger.LogError(e, "Error saving User");
        }

        return isSuccess;
    }

    public async Task<UserDto?> GetUser(Guid id, CancellationToken ctx)
    {
        var user = await _context.Users
                            .AsNoTracking()
                            .Where(user => user.UserId == id)
                            .Select(user => new UserDto
                            {
                                UserId = user.UserId,
                                UserName = user.Email,
                                Password = user.Password,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                UserRoleId = 3,
                                Phone = user.Phone,
                                TinNo = user.TinNo,
                                NICNO = user.NICNO,
                                IsTin = user.IsTin,
                                IsActivePayment = user.IsActivePayment,
                                PackageId = user.PackageId,
                                ProfileImagePath = user.ProfileImagePath,
                                TaxTotal = user.TaxTotal,
                                taxAssistedUserUploadDocsStatus = user.taxAssistedUserUploadDocsStatus
                            })
                            .FirstOrDefaultAsync(ctx);

       

        return user;
    }
    public async Task<bool> UpdateUser(UserDto User)
    {
        bool isSuccess = false;
        // Check if email or phone already exists
        bool userEmailExists = await _context.Users
            .AnyAsync(u => u.Email == User.Email);

        bool userPhoneExists = await _context.Users
           .AnyAsync(u => u.Phone == User.Phone);

        if (userEmailExists)
        {
            Message = "A user with the same Email  already exists.";
            return false; // Do not continue
        }
        if (userPhoneExists)
        {
            Message = "A user with the same Phone number already exists.";
            return false; // Do not continue
        }

        try
        {
            var hasher = new PasswordHasher<object>();
            string hashedPassword = string.Empty;
            if (!string.IsNullOrWhiteSpace(User.Password))
            {
                hashedPassword = hasher.HashPassword(User, User.Password);
            }

            var _user = await _context.Users
                                .Where(p => p.UserId == User.UserId)
                                .FirstOrDefaultAsync();

            if (_user is null)
            {
                Message = "User not found.";
                return false;
            }

            _user.FirstName = User.FirstName;
            _user.LastName = User.LastName;
            _user.Email = User.Email;
            _user.UserName = User.Email;
            _user.Phone = User.Phone;
            _user.NICNO = User.NICNO;
            _user.IsTin = 1;
            _user.TinNo = User.TinNo;
            _user.UpdatedBy = User.UserId.ToString();
            _user.UpdatedOn = DateTime.Now;
            

            await _context.SaveChangesAsync();

            isSuccess = true;
            Message = "Saved successfully";
        }
        catch (Exception e)
        {
            Message = "An error occurred while saving.";
            _logger.LogError(e, "Error saving User");
        }

        return isSuccess;
    }

    public async Task<bool> UpdateUserTinStatus(Guid userId, int tinStatus)
    {
        bool isSuccess = false;

        try
        {
            var _user = await _context.Users
                                .Where(p => p.UserId == userId)
                                .FirstOrDefaultAsync();

            if (_user is null)
            {
                Message = "User not found.";
                return false;
            }
            _user.IsTin = tinStatus;
            _user.UpdatedBy = userId.ToString();
            _user.UpdatedOn = DateTime.Now;


            await _context.SaveChangesAsync();

            isSuccess = true;
            Message = "Saved successfully";
        }
        catch (Exception e)
        {
            Message = "An error occurred while saving.";
            _logger.LogError(e, "Error saving User");
        }

        return isSuccess;
    }

    public async Task<bool> updateProfileImage(string userId, string profileImagePath)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.Users
                                .Where(p => p.UserId.ToString() == userId)
                                .FirstOrDefaultAsync();

            if (_user is null)
            {
                Message = "User not found.";
                return false;
            }
            _user.ProfileImagePath = profileImagePath;
           
            await _context.SaveChangesAsync();

            isSuccess = true;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<bool> UpdateUserUploadedDocumentStatus(Guid userId, int? userUploadedDocStatus)
    {
        bool isSuccess = false;

        try
        {
            var _user = await _context.Users
                                .Where(p => p.UserId == userId)
                                .FirstOrDefaultAsync();

            if (_user is null)
            {
                Message = "User not found.";
                return false;
            }
            _user.taxAssistedUserUploadDocsStatus = userUploadedDocStatus.Value;           
            _user.taxAssistedUserUploadDocsStatusUpdateDate = DateTime.Now;


            await _context.SaveChangesAsync();

            isSuccess = true;
            Message = "Saved successfully";
        }
        catch (Exception e)
        {
            Message = "An error occurred while saving.";
            _logger.LogError(e, "Error saving User");
        }

        return isSuccess;
    }
    public async Task<int?> GetLatestUploadedDocumentStatusAsync(Guid userId)
    {
        return await _context.Users
            .Where(d => d.UserId == userId)
            .Select(d => d.taxAssistedUserUploadDocsStatus )
            .FirstOrDefaultAsync();
    }    

    public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        var hasher = new PasswordHasher<object>();
        string hashedPassword = string.Empty;
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            hashedPassword = hasher.HashPassword(new object(), newPassword);
            user.Password = hashedPassword;
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
