using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TaxFiling.Domain.Entities;
[Table("selfonlineAssets_CapitalCurrentAccount")]
public class SelfonlineAssetsCapitalCurrentAccount
    {
    [Key]
    [Column("SelfonlineBusinessAccountID")]
    public int SelfonlineBusinessAccountID { get; set; }

    [Column("UserId")]
    public string UserId { get; set; }

    [Column("Year")]
    public int Year { get; set; }

    [Column("Type")]
    public string Type { get; set; }

    [Column("SerialNumber")]
    public string SerialNumber { get; set; }

    [Column("BusinessName")]
    public string BusinessName { get; set; }
    [Column("CurrentAccountBalance")]
    public decimal? CurrentAccountBalance { get; set; }
    [Column("CapitalAccountBalance")]
    public decimal? CapitalAccountBalance { get; set; }
}

