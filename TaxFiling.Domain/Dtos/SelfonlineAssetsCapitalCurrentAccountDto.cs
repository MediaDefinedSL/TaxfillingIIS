

namespace TaxFiling.Domain.Dtos;

public class SelfonlineAssetsCapitalCurrentAccountDto
{
    public int SelfonlineBusinessAccountID { get; set; }
    public string UserId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string TransactionType { get; set; }
    public string SerialNumber { get; set; }
    public string BusinessName { get; set; }
    public decimal? CurrentAccountBalance { get; set; }
    public decimal? CapitalAccountBalance { get; set; }
}
