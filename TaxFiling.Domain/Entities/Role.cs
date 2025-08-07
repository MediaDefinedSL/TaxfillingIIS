using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("role")]
public class Role
{
    [Key]
    [Column("code")]
    public int Code { get; set; }
    [Column("role_name")]
    public required string RoleName { get; set; }
    [Column("acronym")]
    public required string Acronym { get; set; }
}
