

namespace TaxFiling.Domain.Dtos;

public class SelfonlineLiabilitiesDisposalAssetsDto
{
    public int SelfonlineDisposalAssetsID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string TransactionType { get; set; }
    public string SerialNumber { get; set; }
    public string Description { get; set; }
    public DateTime? DateOfDisposal { get; set; }
    public decimal? SalesProceed { get; set; }
    public DateTime? DateAcquired { get; set; }
    public decimal? Cost { get; set; }
}
