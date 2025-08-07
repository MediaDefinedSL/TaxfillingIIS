using TaxFiling.Web.Services;
using System.Net;
using System.Net.Http.Headers;

namespace TaxFiling.Web.Handlers;

public class TokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public TokenHandler(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = _httpContextAccessor.HttpContext!.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var userCode = _httpContextAccessor.HttpContext.Request.Cookies["usercode"];
                var tokenModel = await _tokenService.RefreshAccessTokenAsync(userCode!, refreshToken);
                if (!string.IsNullOrEmpty(tokenModel.AccessToken))
                {
                    // Update the access token cookie
                    var accessTokenCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddMinutes(15)
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("access_token", tokenModel.AccessToken, accessTokenCookieOptions);
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("refresh_token", tokenModel.RefreshToken, accessTokenCookieOptions);

                    // Retry the original request with the new access token
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
        }

        return response;
    }
}

