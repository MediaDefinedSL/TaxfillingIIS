namespace TaxFiling.Web.Models
{
    public class OnePayResponseModel
    {
        public OnePayData data { get; set; }
    }

    public class OnePayData
    {
        public string ipg_transaction_id { get; set; }
        public bool status { get; set; } // SUCCESS / FAILED / PENDING
        public string paid_on { get; set; }
        public string transaction_request_datetime { get; set; }
    }
}
