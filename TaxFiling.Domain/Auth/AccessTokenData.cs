namespace TaxFiling.Domain.Auth;

public class AccessTokenData
{
    public string UserName { get; set; } = string.Empty;  
    public int RoleId { get; set; } 
    public string Number { get; set; }
    public string CountryCode { get; set; } 
    public Guid UserId { get; set; }  
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
