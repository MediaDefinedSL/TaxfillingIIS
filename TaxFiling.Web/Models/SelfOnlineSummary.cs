namespace TaxFiling.Web.Models;

public class SelfOnlineSummary
{
    public SelfFilingSummaryCalculationViewModel selfFilingSummaryCalculationViewModel { get; set; }
    public List<SelfonlineAssetsImmovablePropertyViewModel> selfonlineAssetsImmovablePropertyViewModel { get; set; }
    public List<SelfonlineAssetsMotorVehicleViewModel> selfonlineAssetsMotorVehicleViewModel { get; set; }
    public List<SelfOnlineInvestmentIncomeDetailViewModel> selfOnlineInvestmentIncomeDetailViewModel { get; set; }
    public List<SelfonlineAssetsSharesStocksSecuritiesViewModel> selfonlineAssetsSharesStocksSecuritiesViewModel { get; set; }
    public List<SelfonlineAssetsCapitalCurrentAccountViewModel> selfonlineAssetsCapitalCurrentAccountViewModel { get; set; }
    public List<SelfonlineLiabilitiesDisposalAssetsViewModel> selfonlineLiabilitiesDisposalAssets { get; set; }
    public List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel> selfonlineLiabilitiesOtherAssetsGifts { get; set; }
    public List<SelfonlineLiabilitiesAllLiabilitiesViewModel> selfonlineLiabilitiesAllLiabilities { get; set; }
    public List<SelfOnlineEmploymentIncomeDetails> selfOnlineEmploymentIncomeDetails { get; set; }
    public List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel> selfOnlineInvestmentPartnerBeneficiaryExemptViewModel { get; set; }

}
