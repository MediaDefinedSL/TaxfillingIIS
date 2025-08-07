namespace TaxFiling.Web.Models.Auth;

public class TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; init; }
}
