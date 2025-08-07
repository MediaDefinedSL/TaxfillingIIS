using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

public class Entity
{
    [Column("created_by")]
    public string CreatedBy { get; set; }
    [Column("created_on")]
    public DateTime CreatedOn { get; set; }
    [Column("updated_by")]
    public string? UpdatedBy { get; set; }
    [Column("updated_on")]
    public DateTime? UpdatedOn { get; set; }
    [Column("deleted_by")]
    public string? DeletedBy { get; set; }
    [Column("deleted_on")]
    public DateTime? DeletedOn { get; set; }
  
}
