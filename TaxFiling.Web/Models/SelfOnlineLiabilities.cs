namespace TaxFiling.Web.Models;

public class SelfOnlineLiabilities
{
    public List<SelfonlineLiabilitiesDisposalAssetsViewModel> selfonlineLiabilitiesDisposalAssets { get; set; }
    public List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel> selfonlineLiabilitiesOtherAssetsGifts { get; set; }
    public List<SelfonlineLiabilitiesAllLiabilitiesViewModel> selfonlineLiabilitiesAllLiabilities { get; set; }
}
