using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Entities;
[Table("selfonlineflow_personalinformation")]
public class SelfOnlineFlowPersonalInformation 
{
    [Column("selfonlineflow_id")]
    [Key]
    public int SelfOnlineFlowId { get; set; }
    [Column("user_id")]
    public string UserId { get; set; }
    [Column("year")]
    public int Year { get; set; }
    [Column("taxpayer_id")]
    public int? TaxpayerId { get; set; }
    [Column("marital_status_id")]
    public int? MaritalStatusId { get; set; }
    [Column("taxreturn_lastyear_id")]
    public int? TaxReturnLastYearId { get; set; }
    [Column("firstname")]
    public string? FirstName { get; set; }
    [Column("middlename")]
    public string? MiddleName { get; set; }
    [Column("lastname")]
    public string? LastName { get; set; }
    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }
    [Column("tax_number")]
    public string TaxNumber { get; set; }
    [Column("NIC_NO")]
    public string NIC_NO { get; set; }
    [Column("Title")]
    public string? Title { get; set; }
    [Column("PassportNo")]
    public string? PassportNo { get; set; }
    [Column("Nationality")]
    public string? Nationality { get; set; }
    [Column("Occupation")]
    public string? Occupation { get; set; }
    [Column("EmployerName")]
    public string? EmployerName { get; set; }
    [Column("Address")]
    public string Address { get; set; }
    [Column("Gender")]
    public string Gender { get; set; }
    [Column("careof")]
    public string? CareOf { get; set; }
    [Column("apt")]
    public string? Apt { get; set; }
    [Column("street_number")]
    public string? StreetNumber { get; set; }
    [Column("street")]
    public string? Street { get; set; }
    [Column("city")]
    public string? City { get; set; }
    [Column("tel_no_main")]
    public string? TelNoMain { get; set; }
    [Column("tel_no_work")]
    public string? TelNoWork { get; set; }
    [Column("extension")]
    public string? Extension { get; set; }
    [Column("SpouseName")]
    public string SpouseName { get; set; }
    [Column("SpouseTINNo")]
    public string SpouseTINNo { get; set; }
    [Column("SpouseNIC")]
    public string SpouseNIC { get; set; }
    [Column("SomeoneName")]
    public string SomeoneName { get; set; }
    [Column("Relationship")]
    public string Relationship { get; set; }
    [Column("SomeoneTINNo")]
    public string SomeoneTINNo { get; set; }
}
