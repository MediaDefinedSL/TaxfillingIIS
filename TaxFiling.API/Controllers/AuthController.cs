using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaxFiling.Business.Interfaces;
using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxFiling.Domain.Entities;

namespace TaxFiling.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
    {
        var user = await _userRepository.ValidateUser(loginModel);

        if (user == null)
            return NotFound();

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await _userRepository.GenerateRefreshToken(user.UserId);

        var tokenModel = new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RoleID = user.RoleId,
            Email = user.Email,
            IsTin = user.IsTin,
            IsActivePayment = user.IsActivePayment,
            NICNO = user.NICNO,
            TinNo = user.TinNo,
            ProfileImagePath=user.ProfileImagePath,
            UploadedDocumentStatus =user.UploadedDocumentStatus
        };

        return Ok(tokenModel);
    }

    [HttpPost("refresh/token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
    {
        var (isValid, accessTokenData) = await _userRepository.IsValidRefreshToken(refreshTokenModel.UserID, refreshTokenModel.RefreshToken);

        if (!isValid)
            return Unauthorized();

        if (accessTokenData is null)
        {
            return Unauthorized();
        }

        var accessToken = GenerateAccessToken(accessTokenData);
        var refreshToken = await _userRepository.GenerateRefreshToken(accessTokenData.UserId);

        var tokenModel = new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = accessTokenData.UserId,
            FirstName = accessTokenData.FirstName,
            LastName = accessTokenData.LastName,
            RoleID = accessTokenData.RoleId,
            Email = accessTokenData.Email,
            IsTin = accessTokenData.IsTin,
            IsActivePayment = accessTokenData.IsActivePayment,
            NICNO = accessTokenData.NICNO,
            TinNo = accessTokenData.TinNo,
            ProfileImagePath = accessTokenData.ProfileImagePath,
            UploadedDocumentStatus = accessTokenData.UploadedDocumentStatus
        };

        return Ok(tokenModel);
    }

    private string GenerateAccessToken(AccessTokenData accessTokenData)
    {
        var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", accessTokenData.UserId.ToString()),
                    new Claim("UserName", accessTokenData.Email),
                    new Claim("Email", accessTokenData.Email),
                    new Claim("RoleId", accessTokenData.RoleId.ToString()),
                    new Claim("FirstName", accessTokenData.FirstName),
                    new Claim("LastName", accessTokenData.LastName),
                    new Claim("IsTin", accessTokenData.IsTin.ToString()),
                    new Claim("IsActivePayment", accessTokenData.IsActivePayment.ToString()),
                    new Claim("NICNO", accessTokenData.NICNO.ToString() ?? string.Empty),
                    new Claim("TinNo", accessTokenData.TinNo.ToString() ?? string.Empty),
                    new Claim("PackageId", accessTokenData.PackageId.ToString()),
                    new Claim("ProfileImagePath", accessTokenData.ProfileImagePath ?? string.Empty),
                    new Claim("UploadedDocumentStatus", (accessTokenData.UploadedDocumentStatus ?? 0).ToString())
                };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signIn
                    );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }
}
