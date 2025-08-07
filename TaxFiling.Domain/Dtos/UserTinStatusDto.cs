using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class UserTinStatusDto
{
    public Guid UserId { get; set; }
    public int TinStatus { get; set; }
}
