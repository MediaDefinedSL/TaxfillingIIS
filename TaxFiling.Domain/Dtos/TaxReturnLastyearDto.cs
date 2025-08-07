using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class TaxReturnLastyearDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}
