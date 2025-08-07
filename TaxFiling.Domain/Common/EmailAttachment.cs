namespace TaxFiling.Domain.Common;

public class EmailAttachment
{
    public string FileName { get; set; }    // e.g. "invoice.pdf"
    public string ContentType { get; set; } // e.g. "application/pdf"
    public string Base64Data { get; set; }  // Base64-encoded file content
}
