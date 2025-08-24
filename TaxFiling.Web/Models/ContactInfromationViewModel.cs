namespace TaxFiling.Web.Models;

public class ContactInfromationViewModel
{
    public string UserId { get; set; }
    public int Year { get; set; }
    public string? CareOf { get; set; }
    public string? Apt { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public string? EmailPrimary { get; set; }
    public string? EmailSecondary { get; set; }

    public string? MobilePhone { get; set; }
    public string? HomePhone { get; set; }
    public string? WhatsApp { get; set; }

    public string? PreferredCommunicationMethod { get; set; }
}
