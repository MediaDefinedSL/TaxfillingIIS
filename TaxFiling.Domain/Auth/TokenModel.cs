namespace TaxFiling.Domain.Auth;

public sealed class TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; init; }   
    public Guid UserId { get; set; }
    public int RoleID {  get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int IsTin { get; set; }
    public int IsActivePayment { get; set; }
    public string NICNO { get; set; } = string.Empty;
    public string TinNo { get; set; } = string.Empty;
    public int PackageId { get; set; }
    public string ProfileImagePath { get; set; }

    public int? UploadedDocumentStatus { get; set; }
}
