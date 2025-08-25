using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.Common;

namespace TaxFiling.Web.Controllers;

//[Authorize(AuthenticationSchemes = "AuthCookie")]
public class HomeController : Controller
{
    // This action will be the default landing page

    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    private readonly string _baseApiUrl;

    public HomeController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult TaxCalculator()
    {
        return View();
    }
    public IActionResult Blog()
    {
        return View();
    }

    public IActionResult BlogDetails()
    {
        return View();
    }

    public IActionResult NoTinNumber()
    {
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        ViewBag.UserFullName = userName;
        return View();
    }
    public IActionResult FileMyTaxes()
    {
       var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        ViewBag.UserFullName = userName;
        return View();
    }

    public async Task<IActionResult> TaxAssisted(CancellationToken ctx)
    {
        var responseResult = new ResponseResult<object>();
        List<PackagesViewModel> packageses = [];

        var isSelfFiling = 0;

        var queryParams = new Dictionary<string, string?> {
                { "isSelfFiling", isSelfFiling.ToString()}
            };

        //   var client = _httpClientFactory.CreateClient();
        //  var response = await client.GetAsync($"{_baseApiUrl}api/packeges/list" , queryParams);

        string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/packeges/list", queryParams);
        var response = await _httpClient.GetAsync(url, ctx);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent is not null)
            {
                packageses = JsonSerializer.Deserialize<List<PackagesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }
        //  return Json(new { responseResult });
        return View(packageses);
    }

    public IActionResult TaxAssistedPayment()
    {
        return View();
    }
    // Optional: error page
    public IActionResult Error()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }
    
    public async Task<IActionResult> SelfFiling(CancellationToken ctx)
    {

        var responseResult = new ResponseResult<object>();
        List<PackagesViewModel> packageses= [];

        var isSelfFiling = 1;

        var queryParams = new Dictionary<string, string?> {
                { "isSelfFiling", isSelfFiling.ToString()}
            };

     //   var client = _httpClientFactory.CreateClient();
     //  var response = await client.GetAsync($"{_baseApiUrl}api/packeges/list" , queryParams);

        string url = QueryHelpers.AddQueryString($"{_baseApiUrl}api/packeges/list", queryParams);
        var response = await _httpClient.GetAsync(url, ctx);

        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent is not null)
            {
                packageses = JsonSerializer.Deserialize<List<PackagesViewModel>>(responseContent, _jsonSerializerOptions)!;
            }
        }
      //  return Json(new { responseResult });
        return View(packageses);
    }

    public IActionResult SelfOnlineFlow()
    {
        return View();
    }
    public IActionResult SelfOnlineDashboard()
    {
        return View();
    }

    public IActionResult LoadInThisSection()
    {
        return PartialView("_InThisSection");
    }

    public IActionResult LoadTaxPayer()
    {
        return PartialView("_TaxPayerSection");
    }
    public IActionResult LoadMaritalStatus()
    {
        return PartialView("_MaritalStatusSection");
    }

    public IActionResult RefundPolicy()
    {
        return View();
    }

    // GET: /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }
}