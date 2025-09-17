using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("Users")]
public class User : Entity
{
    
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("username")]
    public required string UserName { get; set; }


    [Column("password")]
    public required string Password { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
    [Column("email")]
    public required string Email { get; set; }

    [Column("user_roleId")]
    public int UserRoleId { get; set; }
    [Column("phone")]
    public  string Phone { get; set; }

    [Column("nic_no")]
    public  string NICNO { get; set; }
    [Column("is_tin")]
    public int IsTin { get; set; }
    [Column("tin_no")]
    public string TinNo { get; set; }
    [Column("is_active_payment")]
    public int IsActivePayment { get; set; }

    [Column("profile_imagePath")]
    public string ProfileImagePath { get; set; }
    [Column("TaxTotal")]
    public decimal? TaxTotal { get; set; }

    [Column("is_active")]
    public int IsActive { get; set; }
    [Column("package_id")]
    public int PackageId { get; set; }
    [Column("taxAssistedUserUploadDocsStatus")]
    public int? taxAssistedUserUploadDocsStatus { get; set; }
    [Column("taxAssistedUserUploadDocsStatusUpdateDate")]
    public DateTime? taxAssistedUserUploadDocsStatusUpdateDate { get; set; }
    [Column("IRDPIN")]
    public string IRDPIN {  get; set; }
    [Column("isPersonalInfoCompleted")]
    public int? isPersonalInfoCompleted { get; set; }
    [Column("isIncomeTaxCreditsCompleted")]
    public int isIncomeTaxCreditsCompleted{ get; set; }
}
