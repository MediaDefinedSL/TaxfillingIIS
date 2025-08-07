using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("taxreturn_lastyear")]
public class TaxReturnLastyear : Entity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("css_imagepath")]
    public string ImageUrl { get; set; }

}
