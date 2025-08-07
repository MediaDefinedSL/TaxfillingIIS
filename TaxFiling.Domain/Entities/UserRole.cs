using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("user_role")]
public class UserRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_code")]
    public Guid UserId { get; set; }
    [Column("role_code")]
    public int RoleId { get; set; }
}
