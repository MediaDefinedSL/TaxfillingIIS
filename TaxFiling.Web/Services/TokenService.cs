using TaxFiling.Web.Models.Auth;
using System.Net.Http.Json;

namespace TaxFiling.Web.Services;

public interface ITokenService
{
    Task<TokenModel> RefreshAccessTokenAsync(string userCode, string refreshToken);
}

public class TokenService : ITokenService
{
    public readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<TokenModel> RefreshAccessTokenAsync(string userCode, string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _configuration.GetValue<string>("BaseAPIUrl") + "api/auth/refresh/token",
            new { UserCode = userCode, RefreshToken = refreshToken });
        if (response.IsSuccessStatusCode)
        {
            var tokenModel = await response.Content.ReadFromJsonAsync<TokenModel>();
            return tokenModel;
        }
        return null;
    }
}