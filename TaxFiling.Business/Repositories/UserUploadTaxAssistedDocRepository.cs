using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Repositories;

public class UserUploadTaxAssistedDocRepository : IUserUploadTaxAssistedDocRepository
{
    private readonly Context _context;
    private readonly ILogger<PackagesRepository> _logger;
    private readonly IConfiguration _configuration;
    public UserUploadTaxAssistedDocRepository(Context context, ILogger<PackagesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UserUploadTaxAssistedDocDto>> GetUploadUserList(CancellationToken cancellationToken)
    {
        List<UserUploadTaxAssistedDocDto> uploadUsers = [];
        try
        {

            // Step 1: Get latest uploads per user in memory
            var latestUploads = await _context.UserUploadTaxAssistedDocs
                .GroupBy(t => t.UserId)
                .Select(g => g.OrderByDescending(x => x.UploadDate).FirstOrDefault())
                .ToListAsync(cancellationToken); // Materialize first to avoid projection errors

            // Step 2: Join with Users table in memory
            var users = await _context.Users
                     .Select(u => new
                     {
                         u.UserId,
                         u.FirstName,
                         u.LastName,
                         u.ProfileImagePath,
                         u.Email,
                         u.Phone
                     })
                     .ToListAsync(cancellationToken);

            uploadUsers = (from upload in latestUploads
                           join user in users
                           on upload.UserId equals user.UserId.ToString()
                           select new UserUploadTaxAssistedDocDto
                           {
                               UserUploadId = upload.UserUploadId,
                               UserId = upload.UserId,
                               FullName = user.FirstName + " " + (user.LastName ?? ""),
                               ProfileImagePath = user.ProfileImagePath,
                               Email = user.Email,
                               Phone = user.Phone,
                               Year = upload.Year,
                               UploadDate = upload.UploadDate,
                               CategoryName = upload.CategoryName,
                               T10EmployerName = upload.T10EmployerName,
                               TerminalEmployerName = upload.TerminalEmployerName,
                               AnyOtherType = upload.AnyOtherType,
                               BankConfirmationType = upload.BankConfirmationType,
                               BankName = upload.BankName,
                               UploadedFileName = upload.UploadedFileName,
                               FileName = upload.FileName,
                               Location = upload.Location,
                               DecryptionKey = upload.DecryptionKey,
                               UploadId = upload.UploadId,
                               OriginalName = upload.OriginalName,
                               UploadTime = upload.UploadTime,
                               OtherDocumentName = upload.OtherDocumentName,
                               AssestOptionType = upload.AssestOptionType,
                               AssestsUploadExcelSheetName = upload.AssestsUploadExcelSheetName,
                               AssestType = upload.AssestType,
                               AssetCategory = upload.AssetCategory,
                               AssetNote = upload.AssetNote,
                               AssetVehicleType = upload.AssetVehicleType,
                               AssetInstitution = upload.AssetInstitution
                           }).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return uploadUsers;
    }

    public async Task<int?> SaveUploadedDocsContent(UserUploadTaxAssistedDocDto input)
    {
        try
        {
            var entity = new UserUploadTaxAssistedDoc
            {
                UserId = input.UserId,
                Year = DateTime.Today.Year,
                UploadDate = DateTime.UtcNow,
                CategoryName = input.CategoryName,

                T10EmployerName = input.T10EmployerName,
                TerminalEmployerName = input.TerminalEmployerName,
                AnyOtherType = input.AnyOtherType,
                BankConfirmationType = input.BankConfirmationType,
                BankName = input.BankName,
                UploadedFileName = input.UploadedFileName,

                FileName = input.FileName,
                Location = input.Location,
                DecryptionKey = input.DecryptionKey,
                UploadId = input.UploadId,
                OriginalName = input.OriginalName,
                UploadTime = input.UploadTime,
                OtherDocumentName = input.OtherDocumentName,

                AssestOptionType = input.AssestOptionType,
                AssestsUploadExcelSheetName = input.AssestsUploadExcelSheetName
                 

            };

            _context.UserUploadTaxAssistedDocs.Add(entity);
            await _context.SaveChangesAsync();
            return entity.UserUploadId;
        }
        catch
        {
            // Optionally log the error
            return null;
        }
    }

    public async Task<List<UserUploadTaxAssistedDocDto>> GetUploadedDocsByUser(string userId)
    {
        List<UserUploadTaxAssistedDocDto> uploadedDocs = [];
        try
        {

            uploadedDocs = await _context.UserUploadTaxAssistedDocs
                .Where(t => t.UserId == userId) // ✅ Filter by user
                 .Select(t => new UserUploadTaxAssistedDocDto
                {
                    UserUploadId = t.UserUploadId,
                    UserId = t.UserId,                    
                    UploadDate = t.UploadDate,                    
                    FileName = t.FileName,
                    CategoryName = t.CategoryName,
                    DecryptionKey = t.DecryptionKey ,
                    OriginalName = t.OriginalName,
                    AssetCategory = t.AssetCategory,
                    AssestType = t.AssestType,
                    T10EmployerName = t.T10EmployerName,
                    TerminalEmployerName = t.TerminalEmployerName,
                    AnyOtherType = t.AnyOtherType,
                    BankConfirmationType = t.BankConfirmationType,
                    BankName = t.BankName,
                     AssetNote = t.AssetNote

                 })
                .ToListAsync();

            uploadedDocs = uploadedDocs
            .OrderBy(t => t.CategoryName == "T10" ? 0
                      : t.CategoryName == "Bank Confirmation" ? 1
                      : t.CategoryName == "Terminal Benefit" ? 2
                      : 3) // others after
            .ThenBy(t => t.CategoryName)
            .ThenBy(t => t.UploadDate)
            .ToList();
      }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return uploadedDocs;
    }

    public async Task<bool> DeleteUploadedDocAsync(int userUploadId)
    {
        try
        {
            var entity = await _context.UserUploadTaxAssistedDocs.FindAsync(userUploadId);
            if (entity == null)
                return false;

            _context.UserUploadTaxAssistedDocs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Optionally log the exception
            return false;
        }
    }

    public async Task<int?> SubmitAssetsAsync(List<UserUploadTaxAssistedDocDto> assets)
    {
        try
        {
            foreach (var asset in assets)
            {
                Console.WriteLine("---- Asset ----");
                Console.WriteLine($"AssetType: {asset.AssestType}");
                Console.WriteLine($"AssetCategory: {asset.AssetCategory}");
                Console.WriteLine($"AssetNote: {asset.AssetNote}");
                Console.WriteLine($"AssetVehicleType: {asset.AssetVehicleType}");
                Console.WriteLine($"AssetInstitution: {asset.AssetInstitution}");
                Console.WriteLine($"T10EmployerName: {asset.T10EmployerName}");

                if (asset.Files != null)
                {
                    Console.WriteLine($"Files Count: {asset.Files.Count}");
                    foreach (var file in asset.Files)
                    {
                        Console.WriteLine($"Uploaded File: {file.FileName}, Size: {file.Length}");
                        using var httpClient = new HttpClient();
                        using var content = new MultipartFormDataContent();

                        var stream = file.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        content.Add(fileContent, "file", file.FileName); // Adjust 'files' key if needed by external API


                        var response = await httpClient.PostAsync("https://file.taxfiling.lk/upload", content);
                        response.EnsureSuccessStatusCode();

                        var apiResponse = await response.Content.ReadAsStringAsync();

                        using JsonDocument doc = JsonDocument.Parse(apiResponse);

                        var root = doc.RootElement;

                        bool success = root.GetProperty("success").GetBoolean();
                        string message = root.GetProperty("message").GetString();

                        if (success)
                        {
                            var data = root.GetProperty("data");

                            string filename = data.GetProperty("filename").GetString();
                            string location = data.GetProperty("location").GetString();
                            string decryptionKey = data.GetProperty("decryptionKey").GetString();
                            string uploadId = data.GetProperty("uploadId").GetString();
                            string originalName = data.GetProperty("originalName").GetString();
                            DateTime uploadTime = data.GetProperty("uploadTime").GetDateTime();

                            var entity = new UserUploadTaxAssistedDoc
                            {
                                UserId = asset.UserId,//input.UserId,
                                Year = DateTime.Today.Year,
                                UploadDate = DateTime.UtcNow,
                                CategoryName = "Assets Liabilities",

                                UploadedFileName = originalName,
                                FileName = filename,
                                Location = location,
                                DecryptionKey = decryptionKey,
                                UploadId = uploadId,
                                OriginalName = originalName,
                                UploadTime = uploadTime,

                                AssestOptionType = 2,
                                AssetCategory = asset.AssetCategory,
                                AssetNote = asset.AssetNote,
                                AssetVehicleType = asset.AssetVehicleType,
                                AssetInstitution = asset.AssetInstitution,
                                AssestType = asset.AssestType
                            };

                            _context.UserUploadTaxAssistedDocs.Add(entity);
                            await _context.SaveChangesAsync();
                            //return entity.UserUploadId;

                        }
                        else
                        {
                            return null;

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Files: null");
                    if (!string.IsNullOrWhiteSpace(asset.AssetNote) ||
                        !string.IsNullOrWhiteSpace(asset.AssetVehicleType) ||
                            !string.IsNullOrWhiteSpace(asset.AssetInstitution))
                    {
                        var entity = new UserUploadTaxAssistedDoc
                        {
                            UserId = asset.UserId,//input.UserId,
                            Year = DateTime.Today.Year,
                            UploadDate = DateTime.UtcNow,
                            CategoryName = "Assets Liabilities",

                            AssestOptionType = 2,
                            AssetCategory = asset.AssetCategory,
                            AssetNote = asset.AssetNote,
                            AssetVehicleType = asset.AssetVehicleType,
                            AssetInstitution = asset.AssetInstitution,
                            AssestType = asset.AssestType
                        };

                        _context.UserUploadTaxAssistedDocs.Add(entity);
                        await _context.SaveChangesAsync();

                        // return null;

                    }
                }
            }
            return null;
        }
        catch
        {
            // Optionally log the error
            return null;
        }
    }
    

}
