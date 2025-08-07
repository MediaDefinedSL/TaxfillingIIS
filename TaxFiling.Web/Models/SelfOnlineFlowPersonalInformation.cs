using System.ComponentModel.DataAnnotations;

namespace TaxFiling.Web.Models;

public class SelfOnlineFlowPersonalInformation
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

    public string? CareOf { get; set; }

    public string? Apt { get; set; }

    public string? StreetNumber { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

}
