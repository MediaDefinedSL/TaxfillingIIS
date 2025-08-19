namespace TaxFiling.Web.Models;

public class ContactInfromationViewModel
{
    public string UserId { get; set; }
    public int Year { get; set; }
    public string CareOf { get; set; } = string.Empty;

    public string Apt { get; set; } = string.Empty;

    public string? StreetNumber { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }
}
