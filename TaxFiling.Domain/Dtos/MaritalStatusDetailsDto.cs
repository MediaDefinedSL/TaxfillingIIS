

namespace TaxFiling.Domain.Dtos;

public class MaritalStatusDetailsDto
{
    public string UserId { get; set; }
    public int Year { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string SpouseFullName { get; set; }
    public string SpouseTINNo { get; set; }
    public string SpouseNIC { get; set; }
}
