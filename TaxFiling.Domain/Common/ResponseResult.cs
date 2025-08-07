namespace TaxFiling.Domain.Common;

public sealed class ResponseResult
{
    public int Result { get; set; }
    public Guid ResultGuid { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
    public string ResponseCode { get; set; }
    public string Name { get; set; }
    public List<string> Errors { get; set; } = [];
}
