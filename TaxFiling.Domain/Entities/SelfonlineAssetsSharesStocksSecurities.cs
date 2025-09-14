using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TaxFiling.Domain.Entities;
[Table("selfonlineAssets_sharesstockssecurities")]
public class SelfonlineAssetsSharesStocksSecurities
{
    [Key]
    [Column("SelfonlineSharesStocksID")]
    public int SelfonlineSharesStocksID { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }

    [Column("Year")]
    public int Year { get; set; }

    [Column("Type")]
    public string Type { get; set; }

    [Column("SerialNumber")]
    public string SerialNumber { get; set; }

    [Column("CompanyName")]
    public string CompanyName { get; set; }
    [Column("NoOfSharesStocks")]
    public int? NoOfSharesStocks { get; set; }
    [Column("DateOfAcquisition")]
    public DateTime? DateOfAcquisition { get; set; }
    [Column("CostOfAcquisition")]
    public decimal? CostOfAcquisition { get; set; }
    [Column("NetDividendIncome")]
    public decimal? NetDividendIncome { get; set; }
}
