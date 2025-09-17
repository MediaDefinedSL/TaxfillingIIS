using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.Common;
using TaxFiling.Web.Models.User;

namespace TaxFiling.Web.Controllers;

[Authorize(AuthenticationSchemes = "AuthCookie")]
public class UserUploadTaxAssistedDocController : Controller
{
    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    private readonly string _baseApiUrl;


    public UserUploadTaxAssistedDocController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;
    }

    public async Task<IActionResult> AdminDashboard(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        return View();
    }
    public async Task<IActionResult> TaxAssistedDocUploadUserList(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;


        List<UserUploadTaxAssistedDocViewModel> taxAssistedDocs = [];
        var response1 = await _httpClient.GetAsync($"{_baseApiUrl}api/UserUploadTaxAssistedDoc/uploaduser_list", ctx);
        if (response1 != null && response1.IsSuccessStatusCode)
        {
            var responseContent = await response1.Content.ReadAsStringAsync(ctx);
            if (responseContent is not null)
            {
                taxAssistedDocs = JsonSerializer.Deserialize<List<UserUploadTaxAssistedDocViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }


        return View(taxAssistedDocs);
    }
    public async Task<IActionResult> UserUploadDocuments(string id , CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        UserViewModel user = new();
        //User detail
        if (id != string.Empty)
        {
            var queryParams = new Dictionary<string, string?> {
                { "Id",id }
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
        }

        ViewBag.UserId = $"{user.UserId}";
        ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
        ViewBag.Email = $"{user.Email} ";
        ViewBag.Phone = $"{user.Phone} ";
        ViewBag.ProfileImagePath = $"{user.ProfileImagePath} ";

        return View();
    }

    public async Task<IActionResult> UserUploadDoumentFlow(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;

        return View();
    }


    public async Task<IActionResult> Index(CancellationToken ctx)
    {
        var userId = User.FindFirst("UserID")?.Value;
        var packageId = User.FindFirst("PackageId")?.Value;
        int? latestDocStatus = -1;
        int? personalInfoStatus = -1;
        PackagesViewModel package = new();
        UserViewModel user = new();

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

            var queryParamsDocs = new Dictionary<string, string?>
            {
                { "userId", userId.ToString() }
            };

           
            string docStatusUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/Users/GetUploadedDocStatus", queryParamsDocs);
            var responseDocs = await _httpClient.GetAsync(docStatusUrl, ctx);
            if (responseDocs != null && responseDocs.IsSuccessStatusCode)
            {
                var responseContent = await responseDocs.Content.ReadAsStringAsync(ctx);

                if (int.TryParse(responseContent, out var parsedStatus))
                {
                    latestDocStatus = parsedStatus;
                }
            }
            //string personalInfoStatusUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/Users/GetPersonalInformationCompleted", queryParamsDocs);
            //var responsepersonalInfo = await _httpClient.GetAsync(personalInfoStatusUrl, ctx);
            //if (responsepersonalInfo != null && responsepersonalInfo.IsSuccessStatusCode)
            //{
            //    var responseContent = await responsepersonalInfo.Content.ReadAsStringAsync(ctx);

            //    if (int.TryParse(responseContent, out var parsedPersonalStatus))
            //    {
            //        personalInfoStatus = parsedPersonalStatus;
            //    }
            //}

            var queryUserParams = new Dictionary<string, string?> {
                { "Id", userId.ToString() }
            }; https://localhost:7119/

             string userUrl = QueryHelpers.AddQueryString($"{_baseApiUrl}api/users/getuser", queryUserParams);
            //string userUrl = QueryHelpers.AddQueryString("https://localhost:7119/api/users/getuser", queryUserParams);
            var responseUser = await _httpClient.GetAsync(userUrl, ctx);
            if (responseUser != null && responseUser.IsSuccessStatusCode)
            {
                var responseContent = await responseUser.Content.ReadAsStringAsync(ctx);
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    user = JsonSerializer.Deserialize<UserViewModel>(responseContent, _jsonSerializerOptions) ?? new();
                }
            }


        }

        var uploadedFiles = UploadPage().Result;  // blocking call
        ViewBag.UploadedAssetsFiles = uploadedFiles;        

        ViewBag.packageName = package.Name;
        ViewBag.curancy = package.Curancy;
        ViewBag.price = package.Price;
        ViewBag.UploadedDocStatus = latestDocStatus;
        ViewBag.personalInfoStatus = user.isPersonalInfoCompleted;

        //HttpContext.Session.SetString("IncomeTaxCreditsCompleted", user.isIncomeTaxCreditsCompleted.ToString());
        HttpContext.Session.SetString("personalTaxAssistedComplete", user.isPersonalInfoCompleted.ToString());
        return View();
    }

    public IActionResult UploadDocs()
    {       
        return View();
    }

    private Task<List<UploadedFileViewModel>> UploadPage()
    {
        var files = new List<UploadedFileViewModel>
    {
        new UploadedFileViewModel
        {
            Category = "Deed of land, building, apartment",
            FileName = "deed.pdf",
            UploadedOn = DateTime.Today
        },
        new UploadedFileViewModel
        {
            Category = "Movable Properties (Vehicles)",
            FileName = "bike_reg.pdf",
            UploadedOn = DateTime.Today.AddDays(-1)
        }
    };

        return Task.FromResult(files);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAssets([FromForm] List<string> descriptions,
    [FromForm] List<string> vehicleTypes,
    [FromForm] List<string> vehicleDescriptions,
    [FromForm] List<IFormFile> files)
    {


        var client = _httpClientFactory.CreateClient();

        using var form = new MultipartFormDataContent();

        // Add all descriptions
        foreach (var desc in descriptions)
            form.Add(new StringContent(desc), "descriptions");

        foreach (var type in vehicleTypes)
            form.Add(new StringContent(type), "vehicleTypes");

        foreach (var vehicleDesc in vehicleDescriptions)
            form.Add(new StringContent(vehicleDesc), "vehicleDescriptions");

        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(content, "files", file.FileName);
        }

        // URL of your API project
        var apiUrl = "https://localhost:5001/api/assets/submitassets";

        var response = await client.PostAsync(apiUrl, form);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            TempData["SuccessMessage"] = "✅ Assets submitted successfully!";

            // Redirect to same page or action
            return RedirectToAction("Index", "UserUploadTaxAssistedDoc", new { section = "assets" });
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "API error: " + error);
            return View(); // return error view
        }
    }



    
    


}
