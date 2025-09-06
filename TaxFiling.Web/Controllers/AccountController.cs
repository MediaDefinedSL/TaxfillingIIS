using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Web.Helpers;
using System.Web.WebPages;
using TaxFiling.Web.Models;
using TaxFiling.Web.Models.Auth;
using TaxFiling.Web.Models.Common;
using TaxFiling.Web.Models.User;

namespace TaxFiling.Web.Controllers;

public sealed class AccountController : Controller
{
    public readonly IConfiguration _configuration;
    public readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly HttpClient _httpClient;

    private readonly string _baseApiUrl;

    public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonSerializerOptions)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _baseApiUrl = _configuration.GetValue<string>("BaseAPIUrl") ?? string.Empty;
    }

    #region Login...

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.returnUrl = returnUrl;
        return View();
    }

    [HttpGet]
    public IActionResult Register(string returnUrl)
    {
        ViewBag.returnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UserRegister([FromBody] UserViewModel user)
    {
        bool isFirstName = ModelState.ContainsKey("FirstName") && !ModelState["FirstName"].Errors.Any();
        if (user.FirstName == null )
        {
           
            return View("Register" ,user);
        }
        var responseResult = new ResponseResult<object>();

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_baseApiUrl}api/users/add", user);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent is not null)
            {
                responseResult = JsonSerializer.Deserialize<ResponseResult<object>>(responseContent, _jsonSerializerOptions);
            }
        }
        return Json(new { responseResult });
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody]  LoginModel loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Username))
        {
            return Json(new { Message = "User name cannot be blank" });
        }
        if (string.IsNullOrEmpty(loginModel.Password))
        {
            return Json(new { Message = "Password cannot be blank" });
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_baseApiUrl}api/auth/login", loginModel);
       // var response = await client.PostAsJsonAsync($"https://localhost:7119/api/auth/login", loginModel);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
        
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var tokenModel = JsonSerializer.Deserialize<TokenModel>(responseContent, options);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps
            };

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenModel?.AccessToken);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0";
            var firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value ?? string.Empty;
            var lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value ?? string.Empty;
            var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;
            var isTin = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsTin")?.Value ;
            var isActivePayment = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsActivePayment")?.Value;
            var nicNo = jwtToken.Claims.FirstOrDefault(c => c.Type == "NICNO")?.Value ?? string.Empty;
            var tinNo = jwtToken.Claims.FirstOrDefault(c => c.Type == "TinNo")?.Value ?? string.Empty;
            var packageId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PackageId")?.Value; 
            var profileImagePath = jwtToken.Claims.FirstOrDefault(c => c.Type == "ProfileImagePath")?.Value ?? string.Empty;
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            var userUploadDocumentStatus = jwtToken.Claims.FirstOrDefault(c => c.Type == "UploadedDocumentStatus")?.Value ;


            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
                Console.WriteLine($"Token expires at: {expTime}");
            }

            // Store the token in a secure cookie
            Response.Cookies.Append("access_token", tokenModel?.AccessToken!, cookieOptions);
            Response.Cookies.Append("refresh_token", tokenModel?.RefreshToken!, cookieOptions);

            Response.Cookies.Append("userid", userId!, cookieOptions);

            var fullname = firstName + " " + lastName;
            var claims = new List<Claim> {
                new (ClaimTypes.Name, fullname),
                new Claim("UserID", userId ),
                new Claim("IsTin", isTin ),
                new Claim("IsActivePayment", isActivePayment ),
                new Claim("PackageId", packageId ),
                new Claim("RoleId", roleId ),
                new Claim("ProfileImagePath", profileImagePath ),
                new Claim("TinNo", tinNo),
                new Claim("UserUploadDocStatus",userUploadDocumentStatus)
            };

        
            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("AuthCookie", claimsPrincipal);
            ViewBag.UserId = userId;

            if (roleId == "1")
            {
                return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/UserUploadTaxAssistedDoc/AdminDashboard" });
            }
            else if (isActivePayment== "1" && (!string.IsNullOrWhiteSpace(packageId) && !string.IsNullOrWhiteSpace(packageId)))
            {
                
                 if (packageId == "1" || packageId == "2" || packageId == "3" ||  packageId == "4" || packageId == "5" || packageId == "6")
                {
                    return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/SelfOnlineFlow/SelfOnlineDashboard" });
                }
                    
                //else if (packageId == "4")
                    //return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/UserUploadTaxAssistedDoc" });

                /*  if(roleId == "1") { 

                  }
                  else {
                      //   return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/SelfOnlineFlow/SelfOnlineDashboard" }); UserUploadTaxAssistedDoc/AdminDashboard
                      return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/UserUploadTaxAssistedDoc/AdminDashboard" });
                  }*/

            }
            

        else if (!string.IsNullOrWhiteSpace(nicNo) && !string.IsNullOrWhiteSpace(tinNo))
            {
                return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/Home/FileMyTaxes" });

            }
            else
            {
                return Json(new { success = true, returnUrl = loginModel.ReturnUrl ?? $"/User/UserProfile?userId={userId}" });
            }
        }

        return Json(new { success = false });
    }

    #endregion

    #region Log Out...

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AuthCookie");

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        Response.Cookies.Delete("userId");

        return RedirectToAction("Login");
    }

    #endregion



    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserTinStatus([FromBody] UserTinStatus userTinStatus)
    {
        var responseResult = new ResponseResult<object>();

        var queryParams = new Dictionary<string, string> {
            { "userId", userTinStatus.UserId.ToString() },
            { "tinStatus", userTinStatus.TinStatus.ToString() }
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PutAsJsonAsync($"{_baseApiUrl}api/users/updatetinstatus", queryParams);
        if (response != null && response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (responseContent is not null)
            {
                responseResult = JsonSerializer.Deserialize<ResponseResult<object>>(responseContent, _jsonSerializerOptions);
            }
        }
        return Json(new { responseResult });

    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var client = new HttpClient();

        // API endpoint
        var apiUrl = "https://mail.taxfiling.lk/generate-reset-link";

        // API body (must match your expected parameter name: username)
        var body = new { username = model.Email };

        var response = await client.PostAsJsonAsync(apiUrl, body);

        if (response.IsSuccessStatusCode)
        {
            ViewBag.Message = "We sent a reset link to your email.";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(error);
            string errorText = doc.RootElement.GetProperty("error").GetString();

            ViewBag.Error = $"{errorText} Please try again.";
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token, string email)
    {
        //https://taxfilling.lk/Account/ResetPassword?token=XYZ123&email=user@example.com
        //var model = new ResetPasswordModel { Token = token, Email = email };
        //return View(model);

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            ViewBag.Error = "Invalid reset link.";
            return View("ResetPasswordInvalid"); // Optional: create a separate view for invalid links
        }

        using var client = new HttpClient();
        var apiUrl = $"https://mail.taxfiling.lk/validatetoken/{token}";

        // Call API to validate token
        var response = await client.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Reset token is invalid or has expired.";
            return View("ResetPasswordInvalid"); // Optional: create a separate view for invalid tokens
        }

        var model = new ResetPasswordModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return all validation errors as a single string for AJAX
            var errors = ModelState
         .Where(kvp => kvp.Value.Errors.Count > 0)
         .ToDictionary(
             kvp => kvp.Key,
             kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
         );

            return BadRequest(errors);
        }

        using var client = new HttpClient();
        var apiUrl = $"{_baseApiUrl}api/users/reset-password";

        var body = new
        {
            email = model.Email,
            newPassword = model.Password
        };

        var response = await client.PostAsJsonAsync(apiUrl, body);

        if (response.IsSuccessStatusCode)
        {
            // --- Auto login by calling existing SignIn method ---
            var loginModel = new LoginModel
            {
                Username = model.Email,
                Password = model.Password // raw password, API will verify hash
            };

            var result = await SignIn(loginModel) as IActionResult;

            if (result != null)
            {
                return Ok(new { success = true, message = "Password reset successful! Redirecting to dashboard..." });
            }

            return Ok(new { success = true, message = "Password reset successful, but auto-login failed. Please login manually." });
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return BadRequest(new { success = false, message = $"Something went wrong: {errorContent}" });
    }


}

