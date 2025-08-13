

namespace TaxFiling.Domain.Dtos;

public class TaxPayerDetailsDto
{
    public string UserId { get; set; }
    public int Year { get; set; }
    public int TaxpayerId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string SpouseName { get; set; }
    public string SpouseTINNo { get; set; }
    public string SpouseNIC { get; set; }

    public string SomeoneName { get; set; }
    public string Relationship { get; set; }
    public string SomeoneTINNo { get; set; }
}
