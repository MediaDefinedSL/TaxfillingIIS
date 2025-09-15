using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using TaxFiling.Web.Helpers;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.Common;
using TaxFiling.Web.Models.User;
using TaxFiling.Web.Services;

namespace TaxFiling.Web.Controllers;

[Authorize(AuthenticationSchemes = "AuthCookie")]
public class SelfOnlineFlowController : Controller
{
    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    private readonly string _baseApiUrl;

    private readonly IViewRenderService _viewRenderService;

    public SelfOnlineFlowController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions, IViewRenderService viewRenderService)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;

        _viewRenderService = viewRenderService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> SelfOnlineDashboard(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        var packageId = User.FindFirst("PackageId")?.Value;

        PackagesViewModel package = new();


        if (packageId != null)
        {
            var queryParams = new Dictionary<string, string?> {
                { "id", packageId.ToString()}
            };

            string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/packeges/get", queryParams);
            var response = await _httpClient.GetAsync(url, ctx);
            if (response != null && response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    package = JsonSerializer.Deserialize<PackagesViewModel>(responseContent, _jsonSerializerOptions) ?? new();
                }
            }
        }
        ViewBag.packageName = package.Name;
        ViewBag.packageId = package.PackagesId;

        return View();
    }

    public async Task<IActionResult> SelfOnlineFlow(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        var packageId = User.FindFirst("PackageId")?.Value;
        int year = DateTime.Now.Year;
        var responseResult = false;
        
        // PackagesViewModel package = new();
        UserViewModel user = new();
        ViewBag.TaxTotal = "";

        SelfOnlineFlowPersonalInformation personalInformation = new();


        if (packageId != null)
        {
            //var queryParams = new Dictionary<string, string?> {
            //    { "id", packageId.ToString()}
            //};

            //string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/packeges/get", queryParams);
            //var response = await _httpClient.GetAsync(url, ctx);
            //if (response != null && response.IsSuccessStatusCode)
            //{
            //    var responseContent = await response.Content.ReadAsStringAsync(ctx);
            //    if (!string.IsNullOrWhiteSpace(responseContent))
            //    {
            //        package = JsonSerializer.Deserialize<PackagesViewModel>(responseContent, _jsonSerializerOptions) ?? new();
            //    }
            //}

            var queryParams = new Dictionary<string, string?> {
                { "Id", userId.ToString() }
            };

            string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/users/getuser", queryParams);
            var response = await _httpClient.GetAsync(url, ctx);
            if (response != null && response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    user = JsonSerializer.Deserialize<UserViewModel>(responseContent, _jsonSerializerOptions) ?? new();
                }
            }

            var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

            string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
            var responseuser = await _httpClient.GetAsync(urluser, ctx);
            if (responseuser != null && responseuser.IsSuccessStatusCode)
            {
                var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContentUser))
                {
                    personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
                }
                else
                {
                    string urluserAdd = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/save_useridyear", queryUserParams);
                    var responseusersave = await _httpClient.PostAsync(urluserAdd, null);
                    if (responseusersave != null && responseusersave.IsSuccessStatusCode)
                    {
                        var responseContentUsersave = await responseusersave.Content.ReadAsStringAsync();

                    }
                }
            }

            

            SelfFilingTotalCalculationViewModel totalCalculation = new();




            string url1 = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/get_selfFilingyotalcalculation", queryUserParams);
            var response1 = await _httpClient.GetAsync(url1, ctx);
            if (response1 != null && response1.IsSuccessStatusCode)
            {
                var responseContent1 = await response1.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent1))
                {
                    totalCalculation = JsonSerializer.Deserialize<SelfFilingTotalCalculationViewModel>(responseContent1, _jsonSerializerOptions) ?? new();
                }
            }

        }

        ViewBag.TaxTotal = user.TaxTotal;
        


        return View();
    }
    public async Task<IActionResult> LoadDashboardSection(CancellationToken ctx)
    {
        int? personalInfoStatus = -1;
        var userId = User.FindFirst("UserID")?.Value;
        var queryParamsDocs = new Dictionary<string, string?>
            {
                { "userId", userId.ToString() }
            };

        string personalInfoStatusUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/Users/GetPersonalInformationCompleted", queryParamsDocs);
        var responsepersonalInfo = await _httpClient.GetAsync(personalInfoStatusUrl, ctx);
        if (responsepersonalInfo != null && responsepersonalInfo.IsSuccessStatusCode)
        {
            var responseContent = await responsepersonalInfo.Content.ReadAsStringAsync(ctx);

            if (int.TryParse(responseContent, out var parsedPersonalStatus))
            {
                personalInfoStatus = parsedPersonalStatus;
            }
        }
        ViewBag.personalInfoSelfFilingStatus = personalInfoStatus;
        return PartialView("Partial/_DashboardSection");
    }
    public IActionResult LoadInThisSection()
    {

        return PartialView("Partial/_InThisSection");
    }
    public IActionResult LoadTaxAssistedInThisSection()
    {

        return PartialView("Partial/_InThisSectionTaxAssisted");
    }

    public async Task<IActionResult> LoadTaxPayer(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        SelfOnlineFlowPersonalInformation personalInformation = new();
        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        List<TaxPayerViewModel> taxPayers = [];
        string taxPayersListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/taxpayer_list", queryUserParams);
        //var response1 = await _httpClient.GetAsync($"{_baseApiUrl}api/selfOnlineflow/taxpayer_list", queryUserParams);
        var response1 = await _httpClient.GetAsync(taxPayersListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxPayers = JsonSerializer.Deserialize<List<TaxPayerViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }


        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }
        var taxpayerId = personalInformation.TaxpayerId;
        ViewBag.taxpayer_id = taxpayerId;

        return PartialView("Partial/_TaxPayerSection", taxPayers);
    }
    public async Task<IActionResult> LoadMaritalStatus(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        SelfOnlineFlowPersonalInformation personalInformation = new();
        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        List<MaritalStatusViewModel> maritalStatuses = [];

        string maritalSListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/maritalStatus_list", queryUserParams);
        var response1 = await _httpClient.GetAsync(maritalSListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                maritalStatuses = JsonSerializer.Deserialize<List<MaritalStatusViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }


        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }
        var maritalStatusId = personalInformation.MaritalStatusId;
        ViewBag.maritalStatusId = maritalStatusId;
        return PartialView("Partial/_MaritalStatusSection", maritalStatuses);
    }
    public async Task<IActionResult> LoadTaxReturnLastyear(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        SelfOnlineFlowPersonalInformation personalInformation = new();

        List<TaxReturnLastyear> taxReturnLastyears = [];
        var response1 = await _httpClient.GetAsync($"{_baseApiUrl}api/selfOnlineflow/taxlastyears_list", ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxReturnLastyears = JsonSerializer.Deserialize<List<TaxReturnLastyear>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }
        var taxReturnLastYearId = personalInformation.TaxReturnLastYearId;
        ViewBag.taxReturnLastYearId = taxReturnLastYearId;

        return PartialView("Partial/_TaxReturnLastyear", taxReturnLastyears);
    }
    public async Task<IActionResult> LoadIdentification(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        UserViewModel user = new();
        IdentificationsViewModel identifications = new();
        SelfOnlineFlowPersonalInformation personalInformation = new();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }


        identifications = new IdentificationsViewModel
        {
            UserId = userId,
            Year = year,
            FirstName = personalInformation.FirstName,
            MiddleName = personalInformation.MiddleName,
            LastName = personalInformation.LastName,
            DateOfBirth = personalInformation.DateOfBirth,
            TaxNumber = personalInformation.TaxNumber,
            NIC_NO = personalInformation.NIC_NO,
            Gender = personalInformation.Gender,
            Address = personalInformation.Address,
            Title = personalInformation.Title,
            PassportNo = personalInformation.PassportNo,
            Nationality = personalInformation.Nationality,
            Occupation = personalInformation.Occupation,
            EmployerName = personalInformation.EmployerName
        };

        if (personalInformation.FirstName == null)
        {
            var queryParams = new Dictionary<string, string?> {
                { "Id", userId.ToString()}
            };

            string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/users/getuser", queryParams);
            var response = await _httpClient.GetAsync(url, ctx);
            if (response != null && response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    user = JsonSerializer.Deserialize<UserViewModel>(responseContent, _jsonSerializerOptions) ?? new();

                    identifications = new IdentificationsViewModel
                    {
                        UserId = userId,
                        Year = year,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        TaxNumber = user.TinNo,
                        NIC_NO = user.NICNO
                    };
                }
            }
        }
        return PartialView("Partial/_Identification", identifications);
    }

    public async Task<IActionResult> LoadContactInformation(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        UserViewModel user = new();

        ContactInfromationViewModel contactInfromation = new();
        SelfOnlineFlowPersonalInformation personalInformation = new();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }

        contactInfromation = new ContactInfromationViewModel
        {
            UserId = userId,
            Year = year,
            CareOf = personalInformation.CareOf,
            Apt = personalInformation.Apt,
            StreetNumber = personalInformation.StreetNumber,
            Street = personalInformation.Street,
            City = personalInformation.City,
            District = personalInformation.District,
            PostalCode = personalInformation.PostalCode,
            Country = personalInformation.Country,
            EmailPrimary = personalInformation.EmailPrimary,
            EmailSecondary = personalInformation.EmailSecondary,
            MobilePhone = personalInformation.MobilePhone,
            HomePhone = personalInformation.HomePhone,
            WhatsApp = personalInformation.WhatsApp,
            PreferredCommunicationMethod = personalInformation.PreferredCommunicationMethod
        };

        if (personalInformation.EmailPrimary == null)
        {
            var queryParams = new Dictionary<string, string?> {
                { "Id", userId.ToString()}
            };

            string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/users/getuser", queryParams);
            var response = await _httpClient.GetAsync(url, ctx);
            if (response != null && response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    user = JsonSerializer.Deserialize<UserViewModel>(responseContent, _jsonSerializerOptions) ?? new();

                    contactInfromation = new ContactInfromationViewModel
                    {
                        UserId = userId,
                        Year = year,
                        EmailPrimary = user.Email,
                        MobilePhone = user.Phone
                    };
                }
            }
        }

        return PartialView("Partial/_ContactInformation", contactInfromation);
    }

    public async Task<IActionResult> LoadSummary(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        ContactInfromationViewModel contactInfromation = new();
        SelfOnlineFlowPersonalInformation personalInformation = new();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/sofpersonalinformation_details", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                personalInformation = JsonSerializer.Deserialize<SelfOnlineFlowPersonalInformation>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }

        List<MaritalStatus> maritalStatuses = [];

        string maritalStatus = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/maritalStatus_list", queryUserParams);

        var response1 = await _httpClient.GetAsync(maritalStatus, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                maritalStatuses = JsonSerializer.Deserialize<List<MaritalStatus>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        string? maritalStatusName = null;

        if (personalInformation.MaritalStatusId.HasValue)
        {
            maritalStatusName = maritalStatuses
                .FirstOrDefault(m => m.Id == personalInformation.MaritalStatusId.Value)?.Name;
        }

        ViewBag.MaritalStatusName = maritalStatusName;


        List<TaxPayerViewModel> taxPayers = [];
        string taxPayersListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/taxpayer_list", queryUserParams);

        var response2 = await _httpClient.GetAsync(taxPayersListUrl, ctx);
        if (response2 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response2.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxPayers = JsonSerializer.Deserialize<List<TaxPayerViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }
        string? taxPayerName = null;

        if (personalInformation.TaxpayerId.HasValue)
        {
            taxPayerName = taxPayers
                .FirstOrDefault(m => m.TaxpayerId == personalInformation.TaxpayerId.Value)?.Name;
        }

        ViewBag.TaxPayerName = taxPayerName;

        List<TaxReturnLastyear> taxReturnLastyears = [];
        var response3 = await _httpClient.GetAsync($"{_baseApiUrl}api/selfOnlineflow/taxlastyears_list", ctx);
        if (response3 != null && response3.IsSuccessStatusCode)
        {
            var responseContent = await response3.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxReturnLastyears = JsonSerializer.Deserialize<List<TaxReturnLastyear>>(responseContent, _jsonSerializerOptions)!;
            }
        }
        string? lastyearsName = null;

        if (personalInformation.TaxReturnLastYearId.HasValue)
        {
            lastyearsName = taxReturnLastyears
                .FirstOrDefault(m => m.Id == personalInformation.TaxReturnLastYearId.Value)?.Name;
        }

        ViewBag.LastyearsName = lastyearsName;

        return PartialView("Partial/_SummarySection", personalInformation);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTaxPayerID([FromForm] TaxPayerViewModel taxPayerdetails, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        taxPayerdetails.Year = year;
        taxPayerdetails.UserId = userId;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_taxpayer", taxPayerdetails);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Taxpayer selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMaritalStatus([FromForm] MaritalStatusViewModel maritaldetails, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        maritaldetails.Year = year;
        maritaldetails.UserId = userId;

        var responseResult = new ResponseResult<object>();

        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_maritalstatus", maritaldetails);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "MaritalStatus selected successfully" });
    }
    [HttpPut]
    public async Task<IActionResult> UpdateLastYear(int taxReturnlastyearId, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "lastyearId", taxReturnlastyearId.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/update_lastyear", queryUserParams);
        var response = await _httpClient.PutAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "TaxReturn Last Year selected successfully" });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserIdentifications([FromBody] IdentificationsViewModel userIdentifications)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        userIdentifications.Year = year;
        userIdentifications.UserId = userId;
        var birthday = userIdentifications.DateOfBirth ?? new DateTime(1901, 1, 1);
        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_identification", userIdentifications);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Use rIdentifications selected successfully" });
    }
    [HttpPut]
    public async Task<IActionResult> UpdateContactInfromation([FromBody] ContactInfromationViewModel user)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        user.Year = year;
        user.UserId = userId;


        //  string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/update_contactinformation", user);
        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_contactinformation", user);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Contact Infromation selected successfully" });
    }
    public async Task<IActionResult> PersonalData(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        SelfOnlineFlowPersonalInformation personalInformation = new();

        List<TaxPayer> taxPayers = [];
        var response1 = await _httpClient.GetAsync($"{_baseApiUrl}api/selfOnlineflow/taxpayer_list", ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxPayers = JsonSerializer.Deserialize<List<TaxPayer>>(responseContent, _jsonSerializerOptions)!;
            }
        }


        return PartialView("Partial/_PersonalData", taxPayers);
    }

    //------------------Income and tax Credit 

    public async Task<IActionResult> LoadIncomeLiableTax(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        SelfOnlineEmploymentIncome employmentIncome = new();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/get_employmentincome", queryUserParams);
        var responseuser = await _httpClient.GetAsync(urluser, ctx);
        if (responseuser != null && responseuser.IsSuccessStatusCode)
        {
            var responseContentUser = await responseuser.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContentUser))
            {
                employmentIncome = JsonSerializer.Deserialize<SelfOnlineEmploymentIncome>(responseContentUser, _jsonSerializerOptions) ?? new();
            }

        }

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineEmploymentIncomeDetails> employmentIncomeList = [];
        string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(employmentIncomesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                employmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineEmploymentIncomeDetails>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        var model = new SelfOnlineIncomeLiableTax
        {
            selfOnlineEmploymentIncome = new SelfOnlineEmploymentIncome()
            {
                SelfOnlineEmploymentIncomeId = employmentIncome.SelfOnlineEmploymentIncomeId,
                SeniorCitizen = employmentIncome.SeniorCitizen,
                Residency = employmentIncome.Residency,
                selfOnlineEmploymentIncomeDetails = employmentIncomeList,
                TerminalBenefits = employmentIncome.TerminalBenefits,
                ExemptAmounts = employmentIncome.ExemptAmounts,
                UserId = employmentIncome.UserId,
                Year = employmentIncome.Year,
                Total = employmentIncome.Total
            }
        };
        return PartialView("IncomeTaxPartial/_IncomeLiableTaxSection", model);
    }
    [HttpPost]
    public async Task<IActionResult> AddEmploymentIncome(SelfOnlineEmploymentIncome employmentIncome)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        employmentIncome.UserId = userId;
        employmentIncome.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/save_employmentincome", employmentIncome);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "TaxReturn Last Year selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> AddEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetails employmentIncomeDetails)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        employmentIncomeDetails.UserId = userId;
        employmentIncomeDetails.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/save_employmentincomedetails", employmentIncomeDetails);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "TaxReturn Last Year selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEmploymentIncomeTerminalBenefits(int employmentIncomeId, bool terminalBenefits, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "employmentIncomeId", employmentIncomeId.ToString()},
                { "terminalBenefits", terminalBenefits.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/update_eincometerminalbenefits", queryUserParams);
        var response = await _httpClient.PutAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Terminal Benefits  save successfully" });
    }


    [HttpPost]
    public async Task<IActionResult> UpdateEmploymentIncomeExemptAmounts(int employmentIncomeId, bool exemptAmounts, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "employmentIncomeId", employmentIncomeId.ToString()},
                { "exemptAmounts", exemptAmounts.ToString()}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/update_eincomeexemptamounts", queryUserParams);
        var response = await _httpClient.PutAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Terminal Benefits  save successfully" });
    }

    //---------------box view
    public async Task<IActionResult> LoadEmploymentIncome(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineEmploymentIncomeDetails> employmentIncomeList = [];
        string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(employmentIncomesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                employmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineEmploymentIncomeDetails>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_Incomeliable_EmploymentIncome", employmentIncomeList);
    }

    public async Task<IActionResult> LoadEmploymentDetails(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineEmploymentIncomeDetails> employmentIncomeList = [];
        //string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(employmentIncomesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                employmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineEmploymentIncomeDetails>>(responseContent, _jsonSerializerOptions)!;
            }
        }


        return PartialView("IncomeTaxPartial/_EmploymentDetailsSection", employmentIncomeList);
    }
    public async Task<IActionResult> LoadETerminalBenefits(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineEmploymentIncomeDetails> employmentIncomeList = [];
        string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(employmentIncomesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                employmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineEmploymentIncomeDetails>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_TerminalBenefitsSection", employmentIncomeList);
    }

    public async Task<IActionResult> LoadExemptAmounts(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineEmploymentIncomeDetails> employmentIncomeList = [];
        string employmentIncomesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/employmentincome_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(employmentIncomesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                employmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineEmploymentIncomeDetails>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_ExemptAmountsSection", employmentIncomeList);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetails employmentIncomeDetails)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        employmentIncomeDetails.UserId = userId;
        employmentIncomeDetails.Year = year;

        var responseResult = new ResponseResult<object>(); 

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_employmentincomedetails", employmentIncomeDetails);
        //var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_employmentincomedetails", employmentIncomeDetails);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "TaxReturn Last Year selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEmploymentIncomeDetail(int employmentDetailsId, string employmentDetailsName, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "employmentDetailsId", employmentDetailsId.ToString()},
                 { "employmentDetailsName", employmentDetailsName}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/delete_employmentincomedetail", queryUserParams);
        var response = await _httpClient.PostAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Employment Details  Delete successfully" });
    }

    // -----------------Investment Income

    public async Task<IActionResult> LoadInvestment_Detailsinvestment(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineInvestmentIncomeDetailViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentincomedtail_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentIncomeDetailViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_Investment_DetailsinvestmentSection", investmentIncomeList);
    }
    public async Task<IActionResult> LoadInvestment_PartnerInvestment(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentpartnerbeneficiaryexempt_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_Investment_PartnerInvestmentSection", investmentIncomeList);
    }
    public async Task<IActionResult> LoadInvestment_BeneficiaryInvestment(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentpartnerbeneficiaryexempt_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }
        return PartialView("IncomeTaxPartial/_Investment_BeneficiaryInvestmentSection", investmentIncomeList);
    }
    public async Task<IActionResult> LoadInvestment_ExemptAmounts(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };
        List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentpartnerbeneficiaryexempt_list", queryUserParams1);
        var response1 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        return PartialView("IncomeTaxPartial/_Investment_ExemptAmountsSection", investmentIncomeList);
    }

    [HttpPost]
    public async Task<IActionResult> AddInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDetails investmentIncome)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        investmentIncome.UserId = userId;
        investmentIncome.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/save_investmentincomedetails", investmentIncome);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDetails investmentIncome)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        investmentIncome.UserId = userId;
        investmentIncome.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/save_investmentincomedetails", investmentIncome);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteInvestmentIncomeDetail(int investmentIncomeId, string categoryName, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "investmentIncomeId", investmentIncomeId.ToString()},
                { "categoryName", categoryName}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/delete_investmentincomedetail", queryUserParams);
        var response = await _httpClient.PostAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Employment Details  Delete successfully" });
    }

    //--------new
    [HttpPost]
    public async Task<IActionResult> AddSelfOnlineInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDetailViewModel investmentIncome)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        investmentIncome.UserId = userId;
        investmentIncome.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveinvestment_incomedetails", investmentIncome);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSelfOnlineInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDetailViewModel investmentIncome)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        investmentIncome.UserId = userId;
        investmentIncome.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveinvestment_incomedetails", investmentIncome);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelfOnlineInvestmentIncomeDetails(int investmentIncomeId, string categoryName, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "investmentIncomeId", investmentIncomeId.ToString()},
                { "categoryName", categoryName}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/deleteinvestment_incomedetail", queryUserParams);
        var response = await _httpClient.PostAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Employment Details  Delete successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> AddSelfOnlineInvestmentPartnerBeneficiaryExemptDetails(SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel InvestmentPartnerBeneficiaryExempt)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        InvestmentPartnerBeneficiaryExempt.UserId = userId;
        InvestmentPartnerBeneficiaryExempt.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveinvestment_partnerbeneficiaryexempt", InvestmentPartnerBeneficiaryExempt);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSelfOnlineInvestmentPartnerBeneficiaryExemptDetails(SelfOnlineInvestmentPartnerBeneficiaryExemptViewModel InvestmentPartnerBeneficiaryExempt)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        InvestmentPartnerBeneficiaryExempt.UserId = userId;
        InvestmentPartnerBeneficiaryExempt.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveinvestment_partnerbeneficiaryexempt", InvestmentPartnerBeneficiaryExempt);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteSelfOnlineInvestmentPartnerBeneficiaryExemptDetails(int investmentIncomeId, string categoryName, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "investmentIncomeId", investmentIncomeId.ToString()},
                { "categoryName", categoryName}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/deleteinvestment_partnerbeneficiaryexempt", queryUserParams);
        var response = await _httpClient.PostAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Employment Details  Delete successfully" });
    }


    public async Task<IActionResult> LoadSummarySection(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        SelfFilingSummaryCalculationViewModel totalCalculation = new();

        var queryParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()}
            };

        string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/summarycalculation", queryParams);
        var response = await _httpClient.GetAsync(url, ctx);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                totalCalculation = JsonSerializer.Deserialize<SelfFilingSummaryCalculationViewModel>(responseContent, _jsonSerializerOptions) ?? new();
            }
        }

        List<SelfonlineAssetsImmovablePropertyViewModel> immovablePropertyList = [];
        string immovablePropertyListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetsimmovableproperty_list", queryParams);
        var response1 = await _httpClient.GetAsync(immovablePropertyListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                immovablePropertyList = JsonSerializer.Deserialize<List<SelfonlineAssetsImmovablePropertyViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsMotorVehicleViewModel> motorVehicleList = [];
        string motorVehicleListListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetsmotorvehicle_list", queryParams);
        var response2 = await _httpClient.GetAsync(motorVehicleListListUrl, ctx);
        if (response2 != null && response2.IsSuccessStatusCode)
        {
            var responseContent = await response2.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                motorVehicleList = JsonSerializer.Deserialize<List<SelfonlineAssetsMotorVehicleViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfOnlineInvestmentIncomeDetailViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentincomedtail_list", queryParams);
        var response3 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response3 != null && response3.IsSuccessStatusCode)
        {
            var responseContent = await response3.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentIncomeDetailViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsSharesStocksSecuritiesViewModel> sharesStocksSecuritiesList = [];
        string sharesStocksSecuritiesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetssharesstockssecurities_list", queryParams);
        var response4 = await _httpClient.GetAsync(sharesStocksSecuritiesListUrl, ctx);
        if (response4 != null && response4.IsSuccessStatusCode)
        {
            var responseContent = await response4.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                sharesStocksSecuritiesList = JsonSerializer.Deserialize<List<SelfonlineAssetsSharesStocksSecuritiesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsCapitalCurrentAccountViewModel> capitalCurrentAccountList = [];
        string capitalCurrentAccountListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetscapitalcurrentaccount_list", queryParams);
        var response5 = await _httpClient.GetAsync(capitalCurrentAccountListUrl, ctx);
        if (response5 != null && response5.IsSuccessStatusCode)
        {
            var responseContent = await response5.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                capitalCurrentAccountList = JsonSerializer.Deserialize<List<SelfonlineAssetsCapitalCurrentAccountViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineLiabilitiesAllLiabilitiesViewModel> liabilitiesList = [];
        string liabilitiesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesallliabilities_list", queryParams);
        var response6 = await _httpClient.GetAsync(liabilitiesListUrl, ctx);
        if (response6 != null && response6.IsSuccessStatusCode)
        {
            var responseContent = await response6.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                liabilitiesList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesAllLiabilitiesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel> otherAssetList = [];
        string OtherAssetListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesotherassetss_list", queryParams);
        var response7 = await _httpClient.GetAsync(OtherAssetListUrl, ctx);
        if (response7 != null && response7.IsSuccessStatusCode)
        {
            var responseContent = await response7.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                otherAssetList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineLiabilitiesDisposalAssetsViewModel> disposalAssetsList = [];
        string disposalAssetsListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesdisposalassets_list", queryParams);
        var response8 = await _httpClient.GetAsync(disposalAssetsListUrl, ctx);
        if (response8 != null && response8.IsSuccessStatusCode)
        {
            var responseContent = await response8.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                disposalAssetsList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesDisposalAssetsViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        var model = new SelfOnlineSummary
        {
            selfFilingSummaryCalculationViewModel = totalCalculation,
            selfonlineAssetsImmovablePropertyViewModel = immovablePropertyList,
            selfonlineAssetsMotorVehicleViewModel = motorVehicleList,
            selfOnlineInvestmentIncomeDetailViewModel = investmentIncomeList,
            selfonlineAssetsSharesStocksSecuritiesViewModel = sharesStocksSecuritiesList,
            selfonlineAssetsCapitalCurrentAccountViewModel = capitalCurrentAccountList,
            selfonlineLiabilitiesAllLiabilities = liabilitiesList,
            selfonlineLiabilitiesOtherAssetsGifts = otherAssetList,
            selfonlineLiabilitiesDisposalAssets = disposalAssetsList
        };

        return PartialView("SelfOnlineSummary", model);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadTaxPdf(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        SelfFilingSummaryCalculationViewModel totalCalculation = new();

        var queryParams = new Dictionary<string, string?> {
        { "userId", userId },
        { "year", year.ToString() }
    };

        string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/summarycalculation", queryParams);
        var response = await _httpClient.GetAsync(url, ctx);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                totalCalculation = JsonSerializer.Deserialize<SelfFilingSummaryCalculationViewModel>(
                    responseContent, _jsonSerializerOptions
                ) ?? new();
            }
        }

        var html = await this.RenderViewToStringAsync<object>("~/Views/SelfOnlineFlow/SelfOnlineSummaryPDF.cshtml", totalCalculation);


        using var ms = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, ms);

        //var file = _converter.Convert(pdfDoc);

        return File(ms.ToArray(), "application/pdf", "TaxCalculation.pdf");

    }

    public async Task<IActionResult> LoadDeductions(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var queryUserParams = new Dictionary<string, string?> {
            { "userId", userId.ToString()},
            { "year", year.ToString()}
        };

        SelfFilingTotalCalculationViewModel totalCalculation = new();

        string url1 = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/get_selfFilingyotalcalculation", queryUserParams);
        var response1 = await _httpClient.GetAsync(url1, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent1 = await response1.Content.ReadAsStringAsync(ctx);
            if (!string.IsNullOrWhiteSpace(responseContent1))
            {
                totalCalculation = JsonSerializer.Deserialize<SelfFilingTotalCalculationViewModel>(responseContent1, _jsonSerializerOptions) ?? new();
            }
        }

        return PartialView("IncomeTaxPartial/_Deductions", totalCalculation);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSelfFilingTotalCalculation([FromForm] SelfFilingTotalCalculationViewModel totalCalculation)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        totalCalculation.Year = year;
        totalCalculation.UserId = userId;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/update_selfFilingtotalcalculation", totalCalculation);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Self Filing Total Calculation update successfully" });
    }

    //-------- Assets and Liabilities

    //-------- Assets

    public async Task<IActionResult> LoadAssets(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        ViewBag.userId = userId;

        var queryUserParams = new Dictionary<string, string?> {
            { "userId", userId.ToString()},
            { "year", year.ToString()}
        };


        List<SelfonlineAssetsImmovablePropertyViewModel> immovablePropertyList = [];
        string immovablePropertyListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetsimmovableproperty_list", queryUserParams);
        var response1 = await _httpClient.GetAsync(immovablePropertyListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                immovablePropertyList = JsonSerializer.Deserialize<List<SelfonlineAssetsImmovablePropertyViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsMotorVehicleViewModel> motorVehicleList = [];
        string motorVehicleListListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetsmotorvehicle_list", queryUserParams);
        var response2 = await _httpClient.GetAsync(motorVehicleListListUrl, ctx);
        if (response2 != null && response2.IsSuccessStatusCode)
        {
            var responseContent = await response2.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                motorVehicleList = JsonSerializer.Deserialize<List<SelfonlineAssetsMotorVehicleViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfOnlineInvestmentIncomeDetailViewModel> investmentIncomeList = [];
        string investmentIncomeListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/investmentincomedtail_list", queryUserParams);
        var response3 = await _httpClient.GetAsync(investmentIncomeListUrl, ctx);
        if (response3 != null && response3.IsSuccessStatusCode)
        {
            var responseContent = await response3.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                investmentIncomeList = JsonSerializer.Deserialize<List<SelfOnlineInvestmentIncomeDetailViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsSharesStocksSecuritiesViewModel> sharesStocksSecuritiesList = [];
        string sharesStocksSecuritiesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetssharesstockssecurities_list", queryUserParams);
        var response4 = await _httpClient.GetAsync(sharesStocksSecuritiesListUrl, ctx);
        if (response4 != null && response4.IsSuccessStatusCode)
        {
            var responseContent = await response4.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                sharesStocksSecuritiesList = JsonSerializer.Deserialize<List<SelfonlineAssetsSharesStocksSecuritiesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineAssetsCapitalCurrentAccountViewModel> capitalCurrentAccountList = [];
        string capitalCurrentAccountListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/assetscapitalcurrentaccount_list", queryUserParams);
        var response5 = await _httpClient.GetAsync(capitalCurrentAccountListUrl, ctx);
        if (response5 != null && response5.IsSuccessStatusCode)
        {
            var responseContent = await response5.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                capitalCurrentAccountList = JsonSerializer.Deserialize<List<SelfonlineAssetsCapitalCurrentAccountViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        var model = new SelfOnlineAssets
        {
            selfonlineAssetsImmovablePropertyViewModel = immovablePropertyList,
            selfonlineAssetsMotorVehicleViewModel = motorVehicleList,
            selfOnlineInvestmentIncomeDetailViewModel = investmentIncomeList,
            selfonlineAssetsSharesStocksSecuritiesViewModel = sharesStocksSecuritiesList,
            selfonlineAssetsCapitalCurrentAccountViewModel = capitalCurrentAccountList
        };

        return PartialView("AssetsLiabilities/_Assets", model);
    }


    [HttpPost]
    public async Task<IActionResult> AddEditSelfOnlineImmovableProperty(SelfonlineAssetsImmovablePropertyViewModel immovableProperty)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        immovableProperty.UserId = userId;
        immovableProperty.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveassets_immovableproperties", immovableProperty);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelfOnlineAssetsLiabilitiesDetails(int deleteId, string categoryName, CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        var responseResult = new ResponseResult<object>();

        var queryUserParams = new Dictionary<string, string?> {
                { "userId", userId.ToString()},
                { "year", year.ToString()},
                { "deleteAssetsId", deleteId.ToString()},
                { "categoryName", categoryName}
            };

        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/delete_assetsdetails", queryUserParams);
        var response = await _httpClient.PostAsync(urluser, null);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Delete successfully" });
    }


    [HttpPost]
    public async Task<IActionResult> AddEditSelfOnlineMotorVehicle(SelfonlineAssetsMotorVehicleViewModel motorVehicle)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        motorVehicle.UserId = userId;
        motorVehicle.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveassets_motorVehicles", motorVehicle);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Investment Income selected successfully" });
    }


    [HttpPost]
    public async Task<IActionResult> SaveEditSelfonlineAssetsSharesStocksSecurities(SelfonlineAssetsSharesStocksSecuritiesViewModel sharesStocksSecurities)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        sharesStocksSecurities.UserId = userId;
        sharesStocksSecurities.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveassets_sharesstockssecurities", sharesStocksSecurities);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Shares Stocks Securities save successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditSelfonlineAssetsCapitalCurrentAccount(SelfonlineAssetsCapitalCurrentAccountViewModel capitalCurrentAccount)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        capitalCurrentAccount.UserId = userId;
        capitalCurrentAccount.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/saveassets_capitalcurrentaccount", capitalCurrentAccount);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Capital Current Account save successfully" });
    }


    //-------- Liabilities

    public async Task<IActionResult> LoadLiabilities(CancellationToken ctx)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;
        ViewBag.userId = userId;

        var queryUserParams = new Dictionary<string, string?> {
            { "userId", userId.ToString()},
            { "year", year.ToString()}
        };

        List<SelfonlineLiabilitiesAllLiabilitiesViewModel> liabilitiesList = [];
        string liabilitiesListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesallliabilities_list", queryUserParams);
        var response1 = await _httpClient.GetAsync(liabilitiesListUrl, ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                liabilitiesList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesAllLiabilitiesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel> otherAssetList = [];
        string OtherAssetListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesotherassetss_list", queryUserParams);
        var response2 = await _httpClient.GetAsync(OtherAssetListUrl, ctx);
        if (response2 != null && response2.IsSuccessStatusCode)
        {
            var responseContent = await response2.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                otherAssetList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesOtherAssetsGiftsViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        List<SelfonlineLiabilitiesDisposalAssetsViewModel> disposalAssetsList = [];
        string disposalAssetsListUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/selfOnlineflow/liabilitiesdisposalassets_list", queryUserParams);
        var response3 = await _httpClient.GetAsync(disposalAssetsListUrl, ctx);
        if (response3 != null && response3.IsSuccessStatusCode)
        {
            var responseContent = await response3.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                disposalAssetsList = JsonSerializer.Deserialize<List<SelfonlineLiabilitiesDisposalAssetsViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }

        var model = new SelfOnlineLiabilities
        {
            selfonlineLiabilitiesAllLiabilities = liabilitiesList,
            selfonlineLiabilitiesOtherAssetsGifts = otherAssetList,
            selfonlineLiabilitiesDisposalAssets = disposalAssetsList
        };

        return PartialView("AssetsLiabilities/_Liabilities", model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditSelfonlineLiabilitiesAllLiabilities(SelfonlineLiabilitiesAllLiabilitiesViewModel liabilities)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        liabilities.UserId = userId;
        liabilities.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/savelineLiabilities_allliabilities", liabilities);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Capital Current Account save successfully" });

    }

    [HttpPost]
    public async Task<IActionResult> SaveEditSelfonlineLiabilitiesOtherAssetsGifts(SelfonlineLiabilitiesOtherAssetsGiftsViewModel otherAssets)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        otherAssets.UserId = userId;
        otherAssets.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/savelineLiabilities_otherassetss", otherAssets);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Other Assetss save successfully" });

    }

    [HttpPost]
    public async Task<IActionResult> SaveEditSelfonlineLiabilitiesDisposalAssets(SelfonlineLiabilitiesDisposalAssetsViewModel disposalAssets)
    {

        var userId = User.FindFirst("UserID")?.Value;
        int year = DateTime.Now.Year;

        disposalAssets.UserId = userId;
        disposalAssets.Year = year;

        var responseResult = new ResponseResult<object>();

        // Update user data
        var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/selfOnlineflow/savelineLiabilities_disposalassets", disposalAssets);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }

        return Ok(new { success = true, message = "Disposal Assets save successfully" });

    }
}
