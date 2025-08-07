namespace TaxFiling.Domain.Common;

public class EmailRequest
{
    public string[] ToEmails { get; set; } = [];
    public string[] CcList { get; set; } = [];
    public string Subject { get; set; }
    public string BodyHtml { get; set; }

    public List<EmailAttachment> Attachments { get; set; }
}
