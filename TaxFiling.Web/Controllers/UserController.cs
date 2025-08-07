using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text.Json;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.Common;
using TaxFiling.Web.Models.User;

namespace TaxFiling.Web.Controllers;

[Authorize(AuthenticationSchemes = "AuthCookie")]
public class UserController : Controller
{

    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;
    private readonly string _baseApiUrl;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions, IWebHostEnvironment webHostEnvironment)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile(Guid userId, CancellationToken ctx)
    {
        UserViewModel user = new();

        if (userId != Guid.Empty)
        {
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
        }
        user.BeforeProfileImagePath = user.ProfileImagePath ?? string.Empty;
        ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
        return View(user);
    }

    [HttpPut]
    public async Task<IActionResult> UserUpdate([FromForm] UserViewModel user)
    {
        var responseResult = new ResponseResult<object>();
        var responseResult1 = new ResponseResult<object>();
        // Update user data
        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/users/update", user);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                responseResult = JsonSerializer.Deserialize<ResponseResult<object>>(responseContent, _jsonSerializerOptions);
            }
        }

        
       Console.WriteLine($"ProfileImage Info: Name={user.ProfileImage?.FileName}, Length={user.ProfileImage?.Length}");

        // Upload profile image
        if (user.ProfileImage != null && user.ProfileImage.Length > 0)
        {
            if (user.ProfileImage != null && user.ProfileImage.Length > 0)
            {
                // Build the upload path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserImages", user.UserId.ToString());

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 🔥 Delete old image if it exists
                if (!string.IsNullOrEmpty(user.BeforeProfileImagePath))
                {
                    string existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.BeforeProfileImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }

                // Save new image
                string uniqueFileName = Path.GetFileName(user.ProfileImage.FileName);
                string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await user.ProfileImage.CopyToAsync(stream);
                }

                // Set relative path for storing in DB or API
                string relativePath = $"/UserImages/{user.UserId}/{uniqueFileName}";
                user.ProfileImagePath = relativePath;

                // 🔁 Update profile image path in your backend API
                var queryUserParams1 = new Dictionary<string, string?> {
                { "userId", user.UserId.ToString() },
                { "profileImagePath", user.ProfileImagePath }
            };

                        string urluser = QueryHelpers.AddQueryString($"{_baseApiUrl}api/users/update_profileimage", queryUserParams1);
                        var response1 = await _httpClient.PutAsync(urluser, null);

                        if (response1 != null && response1.IsSuccessStatusCode)
                        {
                            var responseContent1 = await response1.Content.ReadAsStringAsync();
                            // handle if needed
                        }
                    }

        }

        var fullName = $"{user.FirstName} {user.LastName}";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, fullName),
            new Claim("UserID", user.UserId.ToString()),
            new Claim("IsTin", user.IsTin.ToString()),
            new Claim("IsActivePayment", user.IsActivePayment.ToString()),
            new Claim("ProfileImagePath", user.ProfileImagePath ?? string.Empty)
        };

        return Json(new { responseResult });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserTinStatus([FromBody] UserTinStatus userTinStatus)
    {
        var responseResult = new ResponseResult<object>();

        var queryParams = new Dictionary<string, string> {
            { "userId", userTinStatus.UserId.ToString() },
            { "tinStatus", userTinStatus.TinStatus.ToString() }
        };

        var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}api/users/updatetinstatus", queryParams);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                responseResult = JsonSerializer.Deserialize<ResponseResult<object>>(responseContent, _jsonSerializerOptions);
            }
        }

        return Json(new { responseResult });
    }

    [HttpGet]
    public IActionResult DownloadProfileImage(Guid userId, string fileName)
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserImages", userId.ToString());
        string filePath = Path.Combine(folderPath, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("Image not found.");

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "image/png"); // ✅ Correct usage of File()
    }
}
