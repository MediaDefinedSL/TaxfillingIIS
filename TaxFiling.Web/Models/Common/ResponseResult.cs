namespace TaxFiling.Web.Models.Common;

public sealed class ResponseResult<T>
{
    public int Result { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid ResultGuid { get; set; }
    public string Name { get; set; }
    public T? Data { get; set; }
}