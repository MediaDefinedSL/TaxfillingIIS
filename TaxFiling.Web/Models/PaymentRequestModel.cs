namespace TaxFiling.Web.Models;

public class PaymentRequestModel
{
    public string currency { get; set; } = string.Empty;
    public string app_id { get; set; } = string.Empty;
    public string hash { get; set; } = string.Empty;
    public string amount { get; set; } = string.Empty;
    public string reference { get; set; } = string.Empty;
    public string customer_first_name { get; set; } = string.Empty;
    public string customer_last_name { get; set; } = string.Empty;
    public string customer_phone_number { get; set; } = string.Empty;
    public string customer_email { get; set; } = string.Empty;
    public string transaction_redirect_url { get; set; } = string.Empty;
    public string additionalData { get; set; } = string.Empty;
}
