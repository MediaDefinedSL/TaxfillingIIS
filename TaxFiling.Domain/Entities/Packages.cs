using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("packages")]
public class Packages : Entity
{
    [Column("Packages_id")]
    [Key]
    public int PackagesId { get; set; }

    [Column("name")]
    public  string Name { get; set; }

    [Column("description")]
    public  string Description { get; set; }

    [Column("is_selfFiling")]
    public int IsSelfFiling { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; }

    [Column("curancy")]
    public string Curancy { get; set; }

 
}
