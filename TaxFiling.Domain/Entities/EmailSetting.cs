using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;

[Table("email_settings")]
public class EmailSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("mailer_name")]
    public string MailerName { get; set; }
    [Column("driver")]
    public string Driver { get; set; }
    [Column("host")]
    public string Host { get; set; }
    [Column("port")]
    public int Port { get; set; }
    [Column("from_email")]
    public string FromEmail { get; set; }
    [Column("user_name")]
    public string UserName { get; set; }
    [Column("password")]
    public string Password { get; set; }
    [Column("encryption")]
    public string Encryption { get; set; }
    [Column("is_active")]
    public bool IsActive { get; set; }
}
