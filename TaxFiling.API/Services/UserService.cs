using System.Security.Claims;

namespace TaxFiling.API.Services;

public class UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserCode()
    {
        _ = int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);
        return userId;
    }

    public string? GetEmail()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
    }

    public List<string> GetRoles()
    {
        return _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
    }
}
