using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxFiling.Domain.Dtos;

public class SelfOnlineFlowPersonalInformationDto
{
    public int SelfOnlineFlowId { get; set; } 
    public string UserId { get; set; } 
    public int Year { get; set; }
    public int? TaxpayerId { get; set; }
    public int? MaritalStatusId { get; set; }
    public int? TaxReturnLastYearId { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? TaxNumber { get; set; }
    public string? NIC_NO { get; set; }
    public string? Title { get; set; }
    public string? PassportNo { get; set; }
    public string? Nationality { get; set; }
    public string? Occupation { get; set; }
    public string? EmployerName { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? CareOf { get; set; }
    public string? Apt { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string SpouseName { get; set; } = string.Empty;
    public string SpouseTINNo { get; set; } = string.Empty;
    public string SpouseNIC { get; set; } = string.Empty;

    public string SomeoneName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string SomeoneTINNo { get; set; } = string.Empty;
}
