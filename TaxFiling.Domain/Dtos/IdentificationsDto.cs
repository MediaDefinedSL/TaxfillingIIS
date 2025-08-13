
namespace TaxFiling.Domain.Dtos;

public class IdentificationsDto
{
    public string UserId { get; set; }

    public int Year { get; set; }
    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? TaxNumber { get; set; }
    public string? NIC_NO { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
}
