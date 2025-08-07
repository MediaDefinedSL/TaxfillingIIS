namespace TaxFiling.Web.Models;

public class IdentificationsViewModel
{
    public string UserId { get; set; }

    public int Year { get; set; }
    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? TaxNumber { get; set; }
}
