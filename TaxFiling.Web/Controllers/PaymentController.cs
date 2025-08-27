using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.User;
using TaxFiling.Web.Models.User.User;
using TaxFiling.Web.Models.Common;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Text;
using System.Web.Helpers;
using System.Security.Cryptography;

namespace TaxFiling.Web.Controllers;

public class PaymentController : Controller
{
    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    private readonly string _baseApiUrl;


    public PaymentController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;
    }
    public async Task<IActionResult> PackagePayment(int packageId, CancellationToken ctx)
    {
       

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

        return View(package);
    }

    [HttpPost("StartPayment")]
    public async Task<IActionResult> StartPayment(int packageId,decimal Price)
    {
        var client = _httpClientFactory.CreateClient();

        string appId = "3EMG1190963FE17A92690";
        string currency = "LKR";
        string amount = Price.ToString();
        string hashSalt = "DLCY1190963FE17A926B9"; // provided by OnePay

        string input = appId + currency + amount + hashSalt;

        string hash = ComputeSha256Hash(input);

        var paymentRequest = new
        {
            currency = "LKR",
            app_id = "3EMG1190963FE17A92690",
            hash = hash,
            amount = Price.ToString(),
            reference = "REF1750077420233",
            customer_first_name = "testTax",
            customer_last_name = "test",
            customer_phone_number = "0123456789",
            customer_email = "mayusuneth@gmail.com",
            transaction_redirect_url = "https://localhost:7108/Payment/PackageSuccessPage?packageId=" + packageId,
            additionalData = "SubscriptionPlan_1"
        };

        var json = JsonSerializer.Serialize(paymentRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Add("Authorization", "930953613e49f29d11c6560e2aecc8e663bc6d392863764d770ae6cbba0c2cd32418cabd2d865ea0.EUHO1190963FE17A926CE");

        var response = await client.PostAsync("https://api.onepay.lk/v3/checkout/link/", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest($"Error from OnePay: {response.StatusCode} - {responseBody}");
        }

        //using var doc = JsonDocument.Parse(responseBody);
        //var redirectUrl = doc.RootElement.GetProperty("data").GetProperty("gateway").GetProperty("redirect_url").GetString();

        using var doc = JsonDocument.Parse(responseBody);

        var redirectUrl = doc.RootElement.GetProperty("data")
                            .GetProperty("gateway")
                            .GetProperty("redirect_url")
                            .GetString();

        var transaction_id = doc.RootElement.GetProperty("data")
                            .GetProperty("ipg_transaction_id")
                            .GetString();

        return View("PaymentIframe", model: redirectUrl);
        //  return Json(new { redirectUrl });       
    }
    static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // Lowercase hex
            }
            return builder.ToString();
        }
    }

    public async Task<IActionResult> PackageSuccessPage(int packageId, CancellationToken ctx)
    {


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

        return View(package);
    }

    
}
