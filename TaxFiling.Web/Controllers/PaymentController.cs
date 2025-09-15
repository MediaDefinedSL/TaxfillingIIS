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
using System.IO.Pipelines;
using Microsoft.AspNetCore.Http.Json;
using System.Transactions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


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
    public async Task<IActionResult> StartPayment(int packageId, decimal Price)
    {
        UserViewModel user = new();
        //get user details
        var userId = User.FindFirst("UserID")?.Value;
        var userApiUrl = $"{_baseApiUrl}api/users/getuser?id={userId}";

        var userResponse = await _httpClient.GetAsync(userApiUrl);
        if (!userResponse.IsSuccessStatusCode)
        {
            return BadRequest("Failed to fetch user details.");
        }

        var userJson = await userResponse.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(userJson))
        {
            user = JsonSerializer.Deserialize<UserViewModel>(userJson, _jsonSerializerOptions) ?? new();
        }

        var transaction = new Models.UserTransactions
        {
            UserId = userId,  // or from session
            PackageId = packageId,
            TransactionAmount = Price,
            TransactionStatus = 0,        // Pending
            Currency = "LKR",
            TransactionCreatedDate = DateTime.Now,
            OnePayStatus = false
        };

        var transjson = JsonSerializer.Serialize(transaction, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        var OrderResponse = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}api/UserTransactions/SaveTransaction", transaction);

        if (!OrderResponse.IsSuccessStatusCode)
        {
            return StatusCode(500, "Failed to create transaction");
        }

        var responseContent = await OrderResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        int orderId = jsonDoc.RootElement.GetProperty("orderId").GetInt32();

        string orderNumber = "ORDTAX-" + orderId;

        var client = _httpClientFactory.CreateClient();

        string appId = "3EMG1190963FE17A92690";
        string currency = "LKR";
        string amount = Price.ToString();
        string hashSalt = "DLCY1190963FE17A926B9"; // provided by OnePay
        string authorization = "930953613e49f29d11c6560e2aecc8e663bc6d392863764d770ae6cbba0c2cd32418cabd2d865ea0.EUHO1190963FE17A926CE";


        string input = appId + currency + amount + hashSalt;

        string hash = ComputeSha256Hash(input);

        var paymentRequest = new
        {

            currency = currency,
            app_id = appId,
            hash = hash,
            amount = Price.ToString(),
            reference = orderNumber, //"REF1750077420233",
            customer_first_name = user.FirstName,
            customer_last_name = user.LastName,
            customer_phone_number = user.Phone,
            customer_email = user.Email,
            transaction_redirect_url = Url.Action("PackageSuccessPage", "Payment", new { PackageId = packageId, reference = orderNumber }, Request.Scheme),//"https://localhost:7108/Payment/PackageSuccessPage?packageId=" + packageId,
            additionalData = "additional"
        };

        var json = JsonSerializer.Serialize(paymentRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        client.DefaultRequestHeaders.Add("Authorization", authorization);

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

        HttpContext.Session.SetString("LastTransactionId", transaction_id);

        // return View("PaymentIframe", model: redirectUrl);
        return Redirect(redirectUrl);
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

    [HttpGet]
    public async Task<IActionResult> PackageSuccessPage(int PackageId, string reference)
    {
        // Save PackageId and reference for the view
        ViewBag.PackageId = PackageId;
        ViewBag.Reference = reference;

        // Get transaction ID from session
        string transactionId = HttpContext.Session.GetString("LastTransactionId");
        if (string.IsNullOrEmpty(transactionId))
            return View("Error");

        // Prepare OnePay request
        string url = "https://api.onepay.lk/v3/transaction/status/";
        string appId = "3EMG1190963FE17A92690";
        string authorization = "930953613e49f29d11c6560e2aecc8e663bc6d392863764d770ae6cbba0c2cd32418cabd2d865ea0.EUHO1190963FE17A926CE";

        var requestBody = new
        {
            app_id = appId,
            onepay_transaction_id = transactionId
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", authorization);
        using var content = JsonContent.Create(requestBody);

        // Call OnePay API
        var response = await client.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
            return View("Error");

        var responseJson = await response.Content.ReadAsStringAsync();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var onePayResponse = JsonSerializer.Deserialize<OnePayResponseModel>(responseJson, jsonOptions);

        if (onePayResponse == null)
            return View("Error");

        // Extract order ID from reference
        var orderId = reference.Substring(7);

        // Update transaction status via API
        var updateTransaction = new
        {
            OrderId = orderId,
            OnePayTransactionId = onePayResponse.data.ipg_transaction_id,
            OnePayStatus = onePayResponse.data.status,
            OnePayPaidOn = onePayResponse.data.paid_on,
            OnePayTransactionRequestDatetime = onePayResponse.data.transaction_request_datetime,
            PackageId = PackageId
        };

        var updateResponse = await client.PostAsJsonAsync(
            $"{_baseApiUrl}api/UserTransactions/UpdatePaymentStatus",
            updateTransaction
        );

        if (!updateResponse.IsSuccessStatusCode)
            return View("Error");

        // If payment failed, redirect to fail page
        if (!onePayResponse.data.status)
            return RedirectToAction("PackageFailPage", "Payment", new { orderId , PackageId});

        // Fetch transaction details from API
        var orderDataResponse = await client.GetAsync($"{_baseApiUrl}api/UserTransactions/GetTransactionById/{orderId}");
        if (!orderDataResponse.IsSuccessStatusCode)
            return View("Error");

        var jsonOrderData = await orderDataResponse.Content.ReadAsStringAsync();
        var transaction = JsonSerializer.Deserialize<UserTransactionViewModel>(jsonOrderData, jsonOptions);

        if (transaction == null)
            return View("Error");

        // ✅ Refresh Claims with IsActivePayment updated
        var userId = User.FindFirst("UserID")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            // Get latest user data from API
            var userResponse = await client.GetAsync($"{_baseApiUrl}api/Users/getuser?id={userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var jsonUser = await userResponse.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserViewModel>(jsonUser, jsonOptions);

                if (user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim("UserID", user.UserId.ToString()),
                    new Claim("IsTin", user.IsTin.ToString()),
                    new Claim("IsActivePayment", user.IsActivePayment.ToString()), // ✅ updated
                    new Claim("ProfileImagePath", user.ProfileImagePath ?? string.Empty),
                    new Claim("PackageId", PackageId.ToString()),
                    new Claim("UserUploadDocStatus","0"),
                    new Claim("TinNo", user.TinNo),
                    new Claim("RoleId", user.UserRoleId.ToString() )
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        "AuthCookie",
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties { IsPersistent = true }
                    );
                }
            }
        }

        // Return success view with transaction data
        return View(transaction);
    }


    public async Task<IActionResult> DownloadReceipt(int orderId)
    {
        // Fetch transaction details
        var response = await _httpClient.GetAsync($"{_baseApiUrl}api/UserTransactions/GetTransactionById/{orderId}");
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var transaction = JsonSerializer.Deserialize<UserTransactionViewModel>(json, jsonOptions);

        if (transaction == null)
            return NotFound();

        // Create PDF
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        PdfPTable headerTable = new PdfPTable(2)
        {
            WidthPercentage = 100
        };
        headerTable.SetWidths(new float[] { 1, 3 }); // adjust column ratio

        // Left cell: Logo
        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "Tax-new-8-edit-one.png");
        if (System.IO.File.Exists(logoPath))
        {
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(80f, 80f); // adjust size
            PdfPCell logoCell = new PdfPCell(logo)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headerTable.AddCell(logoCell);
        }
        else
        {
            // Empty cell if logo missing
            headerTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });
        }

        // Right cell: Company Name
        var companyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.Black);
        PdfPCell nameCell = new PdfPCell(new Phrase("TaxFilling (PVT) Ltd", companyFont))
        {
            Border = Rectangle.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_MIDDLE
        };
        headerTable.AddCell(nameCell);

        // Add header table to document
        document.Add(headerTable);

        // Add spacing after header
        document.Add(new Paragraph("\n"));


        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
        var heading = new Paragraph("Payment Receipt", titleFont) { Alignment = Element.ALIGN_CENTER };
        document.Add(heading);

        document.Add(new Paragraph("\n")); // spacing

        // Add status
        var statusFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.Green);
        if (transaction.TransactionStatus != 1)
            statusFont.Color = BaseColor.Red;

        var statusText = transaction.TransactionStatus == 1 ? "Payment Success" : "Payment Failed";
        var statusParagraph = new Paragraph(statusText, statusFont) { Alignment = Element.ALIGN_CENTER };
        document.Add(statusParagraph);

        document.Add(new Paragraph("\n")); // spacing

        // Add transaction info table
        PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1, 2 }); // Column widths

        void AddCell(string label, string value)
        {
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            table.AddCell(new PdfPCell(new Phrase(label, boldFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(value, regularFont)) { Border = Rectangle.NO_BORDER });
        }

        AddCell("Order ID:", transaction.OrderId.ToString());
        AddCell("Order Number:", transaction.OrderNumber);
        AddCell("User Full Name:", transaction.UserFullName ?? "-");
        AddCell("Package Name:", transaction.PackageName ?? "-");
        AddCell("Amount:", $"{transaction.Currency} {transaction.TransactionAmount}");
        AddCell("Created On:", transaction.TransactionCreatedDate.ToString("yyyy-MM-dd HH:mm"));
        AddCell("Paid On:", transaction.OnePayPaidOn ?? "-");
        AddCell("OnePay Transaction ID:", transaction.OnePayTransactionId ?? "-");
        AddCell("Status:", transaction.TransactionStatus == 1 ? "Success" : "Failed");

        document.Add(table);

        // Optional: Add footer or thank you note
        document.Add(new Paragraph("\nThank you for your payment!", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 12)));

        document.Close();

        byte[] fileBytes = memoryStream.ToArray();
        return File(fileBytes, "application/pdf", $"Receipt_{transaction.OrderId}.pdf");
    }

    public IActionResult PackageFailPage(int orderId, int packageId)
    {
        ViewBag.OrderId = orderId;
        ViewBag.PackageId = packageId;
        return View();
    }


}