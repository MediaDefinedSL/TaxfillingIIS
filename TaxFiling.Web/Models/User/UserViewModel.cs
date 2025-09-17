using System.ComponentModel.DataAnnotations;


namespace TaxFiling.Web.Models.User;

public class UserViewModel
{
  
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First Name1 is required.")]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string NICNO { get; set; }
    public int IsTin { get; set; }
    public string TinNo { get; set; }
    public int IsActivePayment { get; set; }
    public int UserRoleId { get; set; }
    public string ProfileImagePath { get; set; }
    public decimal? TaxTotal { get; set; }
    public bool IsActive { get; set; }
    public int PackageId { get; set; }
    public string BeforeProfileImagePath { get; set; }

    public IFormFile? ProfileImage { get; set; }
    public int? taxAssistedUserUploadDocsStatus { get; set; }

    public string IRDPIN { get; set; }
    public int? isPersonalInfoCompleted { get; set; }

    public int isIncomeTaxCreditsCompleted { get; set; }

}
