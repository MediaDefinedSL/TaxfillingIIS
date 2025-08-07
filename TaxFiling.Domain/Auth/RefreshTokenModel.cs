namespace TaxFiling.Domain.Auth;

public class RefreshTokenModel
{
    public Guid UserID { get; set; }
    public string RefreshToken { get; init; } = string.Empty;
}
