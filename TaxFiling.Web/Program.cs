using Microsoft.AspNetCore.Authentication.Cookies;
using TaxFiling.Web.Handlers;
using TaxFiling.Web.Services;
using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Extensions; // Needed for UriHelper
using TaxFiling.Web.Services;
using DinkToPdf.Contracts;
using DinkToPdf;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try
{
    Log.Information("Starting application");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}

builder.Host.UseSerilog();
builder.Services.AddDistributedMemoryCache(); // Needed for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// Configure JsonSerializerOptions as a singleton
builder.Services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

builder.Services.AddTransient<TokenHandler>();
builder.Services.AddHttpClient("ApiClient")
    .AddHttpMessageHandler<TokenHandler>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Cookie timeout minutes
var cookieTimeoutMinutes = int.Parse(builder.Configuration["cookieTimeoutMinutes"]?.ToString() ?? "10");

// Add authentication with cookies
builder.Services
    .AddAuthentication("AuthCookie") // Scheme name
    .AddCookie("AuthCookie", options =>
    {
        options.Cookie.Name = "UserAuthCookie";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ Enforce HTTPS-only cookies
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if unauthorized
        options.Cookie.Name = "MyAppAuth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieTimeoutMinutes); // Set expiration time

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // 🔒 Force HTTPS in redirect URL manually
                var redirectUri = UriHelper.BuildAbsolute(
                    "https", // Force the scheme
                    context.Request.Host,
                    context.Request.PathBase,
                    context.RedirectUri
                );

                Console.WriteLine($"[Login Redirect] Redirecting to: {redirectUri}");

                context.Response.Redirect(redirectUri);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedUsers", policy =>
        policy.RequireAuthenticatedUser());





builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddSingleton<IViewRenderService, ViewRenderService>();

var app = builder.Build();



// ✅ Forwarded headers — must come BEFORE UseHttpsRedirection
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
    ForwardLimit = 1,
    RequireHeaderSymmetry = false
});

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // ✅ Redirect HTTP to HTTPS
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
