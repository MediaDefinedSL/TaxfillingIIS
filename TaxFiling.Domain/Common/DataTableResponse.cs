namespace TaxFiling.Domain.Common;

public sealed class DataTableResponse<T>
{
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public List<T> Data { get; set; }

    public DataTableResponse()
    {
        Data = [];
    }
}
