using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("refresh_token")]
public class UserRefreshToken
{
    [Key]
    public Guid UserId { get; set; }
    [Column("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
