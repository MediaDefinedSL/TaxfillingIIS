using Microsoft.AspNetCore.Http;
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
                     DecryptionKey = t.DecryptionKey,
                     OriginalName = t.OriginalName,
                     AssetCategory = t.AssetCategory,
                     AssestType = t.AssestType,
                     T10EmployerName = t.T10EmployerName,
                     TerminalEmployerName = t.TerminalEmployerName,
                     AnyOtherType = t.AnyOtherType,
                     BankConfirmationType = t.BankConfirmationType,
                     BankName = t.BankName,
                     AssetNote = t.AssetNote,
                     AssetInstitution = t.AssetInstitution,
                     AssetVehicleType = t.AssetVehicleType,
                     OtherDocumentName = t.OtherDocumentName,
                     UploadId = t.UploadId

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

    //public async Task<int?> SubmitAssetsAsync(List<UserUploadTaxAssistedDocDto> assets)
    //{
    //    try
    //    {
    //        foreach (var asset in assets)
    //        {
    //            Console.WriteLine("---- Asset ----");
    //            Console.WriteLine($"AssetType: {asset.AssestType}");
    //            Console.WriteLine($"AssetCategory: {asset.AssetCategory}");
    //            Console.WriteLine($"AssetVehicleType: {asset.AssetVehicleType}");
    //            Console.WriteLine($"AssetInstitution: {asset.AssetInstitution}");
    //            Console.WriteLine($"T10EmployerName: {asset.T10EmployerName}");

    //            // Pair notes and files manually
    //            var pairedItems = new List<AssetUploadItem>();
    //            var notes = asset.AssetNotes ?? new List<string>();
    //            var files = asset.Files ?? new List<IFormFile>();                
    //            var usedNoteIndexes = new HashSet<int>();
    //            int noteCount = asset.AssetNotes?.Count ?? 0;
    //            int fileCount = asset.Files?.Count ?? 0;
    //            int pairCount = Math.Min(noteCount, fileCount);

    //            var unpairedNotes = new List<AssetUploadItem>();
    //            var unpairedFiles = new List<AssetUploadItem>();
    //            var vehicleTypes = asset.AssetVehicleTypes ?? new List<string>();
    //            var institutions = asset.AssetInstitutions ?? new List<string>();

    //            var maxCount = Math.Max(fileCount, noteCount);
    //            if (asset.AssetCategory != "Cash in hand" && asset.AssetCategory != "Loans given & amount receivable" && asset.AssetCategory != "Value of gold, silver, gems, jewellery" && asset.AssetCategory != "Disposal of assets including shares")
    //            {
    //                for (int i = 0; i < maxCount; i++)
    //                {
    //                    var hasFile = i < fileCount && files[i] != null;
    //                    var hasNote = i < noteCount && !string.IsNullOrWhiteSpace(notes[i]);

    //                    var vehicleType = i < vehicleTypes.Count ? vehicleTypes[i] : null;
    //                    var institution = i < institutions.Count ? institutions[i] : null;
    //                    var note = i < notes.Count ? notes[i] : null;
    //                    var file = i < files.Count ? files[i] : null;

    //                    if (hasFile && hasNote)
    //                    {
    //                        pairedItems.Add(new AssetUploadItem
    //                        {
    //                            AssetNote = note,
    //                            File = file,
    //                            AssetVehicleType = vehicleType,
    //                            AssetInstitution= institution
    //                        });
    //                        // usedNoteIndexes.Add(i);
    //                    }
    //                    else if (hasNote)
    //                    {
    //                        // ❗ Note exists without a file
    //                        unpairedNotes.Add(new AssetUploadItem
    //                        {
    //                            AssetNote = notes[i],
    //                            AssetVehicleType = vehicleType,
    //                            AssetInstitution = institution
    //                        });
    //                    }
    //                    else if (hasFile)
    //                    {
    //                        // ❗ File exists without a note
    //                        unpairedFiles.Add(new AssetUploadItem
    //                        {
    //                            File = files[i],
    //                            AssetVehicleType = vehicleType,
    //                            AssetInstitution = institution
    //                        });
    //                    }
    //                }
    //            }

    //            if (pairedItems.Any())
    //            {
    //                foreach (var item in pairedItems)
    //                {
    //                    using var httpClient = new HttpClient();
    //                    using var content = new MultipartFormDataContent();

    //                    using var stream = item.File.OpenReadStream();
    //                    var fileContent = new StreamContent(stream);
    //                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.File.ContentType);
    //                    content.Add(fileContent, "file", item.File.FileName);

    //                    var response = await httpClient.PostAsync("http://129.213.51.109:3001/upload", content);
    //                    response.EnsureSuccessStatusCode();

    //                    var apiResponse = await response.Content.ReadAsStringAsync();
    //                    using JsonDocument doc = JsonDocument.Parse(apiResponse);
    //                    var root = doc.RootElement;

    //                    if (!root.GetProperty("success").GetBoolean())
    //                        return null;

    //                    var data = root.GetProperty("data");
    //                    string filename = data.GetProperty("filename").GetString();
    //                    string location = data.GetProperty("location").GetString();
    //                    string decryptionKey = data.GetProperty("decryptionKey").GetString();
    //                    string uploadId = data.GetProperty("uploadId").GetString();
    //                    string originalName = data.GetProperty("originalName").GetString();
    //                    DateTime uploadTime = data.GetProperty("uploadTime").GetDateTime();

    //                    var entity = new UserUploadTaxAssistedDoc
    //                    {
    //                        UserId = asset.UserId,
    //                        Year = DateTime.Today.Year,
    //                        UploadDate = DateTime.UtcNow,
    //                        CategoryName = "Assets Liabilities",
    //                        AssestOptionType = 2,
    //                        AssetCategory = asset.AssetCategory,
    //                        AssetNote = item.AssetNote,
    //                        AssetVehicleType = item.AssetVehicleType,
    //                        AssetInstitution = item.AssetInstitution,
    //                        AssestType = asset.AssestType,

    //                        UploadedFileName = originalName,
    //                        FileName = filename,
    //                        Location = location,
    //                        DecryptionKey = decryptionKey,
    //                        UploadId = uploadId,
    //                        OriginalName = originalName,
    //                        UploadTime = uploadTime
    //                    };

    //                    _context.UserUploadTaxAssistedDocs.Add(entity);
    //                }

    //                await _context.SaveChangesAsync();

    //                if (unpairedNotes.Any())
    //                {
    //                    foreach (var item in unpairedNotes)                        
    //                   {
    //                        var entity = new UserUploadTaxAssistedDoc
    //                        {
    //                            UserId = asset.UserId,
    //                            Year = DateTime.Today.Year,
    //                            UploadDate = DateTime.UtcNow,
    //                            CategoryName = "Assets Liabilities",
    //                            AssestOptionType = 2,
    //                            AssetCategory = asset.AssetCategory,
    //                            AssetNote = item.AssetNote,
    //                            AssetVehicleType = item.AssetVehicleType,
    //                            AssetInstitution = item.AssetInstitution,
    //                            AssestType = asset.AssestType
    //                        };

    //                        _context.UserUploadTaxAssistedDocs.Add(entity);
    //                    }

    //                    await _context.SaveChangesAsync();
    //                }

    //                if (unpairedFiles.Any() )
    //                {
    //                    foreach (var item in unpairedFiles)
    //                    {
    //                        using var httpClient = new HttpClient();
    //                        using var content = new MultipartFormDataContent();

    //                        using var stream = item.File.OpenReadStream();
    //                        var fileContent = new StreamContent(stream);
    //                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.File.ContentType);
    //                        content.Add(fileContent, "file", item.File.FileName);

    //                        var response = await httpClient.PostAsync("http://129.213.51.109:3001/upload", content);
    //                        response.EnsureSuccessStatusCode();

    //                        var apiResponse = await response.Content.ReadAsStringAsync();
    //                        using JsonDocument doc = JsonDocument.Parse(apiResponse);
    //                        var root = doc.RootElement;

    //                        if (!root.GetProperty("success").GetBoolean())
    //                            return null;

    //                        var data = root.GetProperty("data");
    //                        string filename = data.GetProperty("filename").GetString();
    //                        string location = data.GetProperty("location").GetString();
    //                        string decryptionKey = data.GetProperty("decryptionKey").GetString();
    //                        string uploadId = data.GetProperty("uploadId").GetString();
    //                        string originalName = data.GetProperty("originalName").GetString();
    //                        DateTime uploadTime = data.GetProperty("uploadTime").GetDateTime();

    //                        var entity = new UserUploadTaxAssistedDoc
    //                        {
    //                            UserId = asset.UserId,
    //                            Year = DateTime.Today.Year,
    //                            UploadDate = DateTime.UtcNow,
    //                            CategoryName = "Assets Liabilities",
    //                            AssestOptionType = 2,
    //                            AssetCategory = asset.AssetCategory,
    //                            AssetVehicleType = item.AssetVehicleType,
    //                            AssetInstitution = item.AssetInstitution,
    //                            AssestType = asset.AssestType,

    //                            UploadedFileName = originalName,
    //                            FileName = filename,
    //                            Location = location,
    //                            DecryptionKey = decryptionKey,
    //                            UploadId = uploadId,
    //                            OriginalName = originalName,
    //                            UploadTime = uploadTime
    //                        };

    //                        _context.UserUploadTaxAssistedDocs.Add(entity);
    //                    }

    //                    await _context.SaveChangesAsync();
    //                }
    //            }
    //            else
    //            {                   
    //                    if(files.Count > 0)
    //                    {
    //                        foreach (var item in files)
    //                        {
    //                            using var httpClient = new HttpClient();
    //                            using var content = new MultipartFormDataContent();

    //                            using var stream = item.OpenReadStream();
    //                            var fileContent = new StreamContent(stream);
    //                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
    //                            content.Add(fileContent, "file", item.FileName);

    //                            var response = await httpClient.PostAsync("http://129.213.51.109:3001/upload", content);
    //                            response.EnsureSuccessStatusCode();

    //                            var apiResponse = await response.Content.ReadAsStringAsync();
    //                            using JsonDocument doc = JsonDocument.Parse(apiResponse);
    //                            var root = doc.RootElement;

    //                            if (!root.GetProperty("success").GetBoolean())
    //                                return null;

    //                            var data = root.GetProperty("data");
    //                            string filename = data.GetProperty("filename").GetString();
    //                            string location = data.GetProperty("location").GetString();
    //                            string decryptionKey = data.GetProperty("decryptionKey").GetString();
    //                            string uploadId = data.GetProperty("uploadId").GetString();
    //                            string originalName = data.GetProperty("originalName").GetString();
    //                            DateTime uploadTime = data.GetProperty("uploadTime").GetDateTime();

    //                            var entity = new UserUploadTaxAssistedDoc
    //                            {
    //                                UserId = asset.UserId,
    //                                Year = DateTime.Today.Year,
    //                                UploadDate = DateTime.UtcNow,
    //                                CategoryName = "Assets Liabilities",
    //                                AssestOptionType = 2,
    //                                AssetCategory = asset.AssetCategory,
    //                                AssetVehicleType = asset.AssetVehicleType,
    //                                AssetInstitution = asset.AssetInstitution,
    //                                AssestType = asset.AssestType,

    //                                UploadedFileName = originalName,
    //                                FileName = filename,
    //                                Location = location,
    //                                DecryptionKey = decryptionKey,
    //                                UploadId = uploadId,
    //                                OriginalName = originalName,
    //                                UploadTime = uploadTime
    //                            };

    //                            _context.UserUploadTaxAssistedDocs.Add(entity);
    //                        }

    //                        await _context.SaveChangesAsync();

    //                    }
    //                    if (notes.Count > 0 && notes[0] != null)
    //                    {

    //                        var entity = new UserUploadTaxAssistedDoc
    //                        {
    //                            UserId = asset.UserId,
    //                            Year = DateTime.Today.Year,
    //                            UploadDate = DateTime.UtcNow,
    //                            CategoryName = "Assets Liabilities",
    //                            AssestOptionType = 2,
    //                            AssetCategory = asset.AssetCategory,
    //                            AssetVehicleType = asset.AssetVehicleType,
    //                            AssetInstitution = asset.AssetInstitution,
    //                            AssestType = asset.AssestType,
    //                            AssetNote = notes[0],
    //                        };

    //                        _context.UserUploadTaxAssistedDocs.Add(entity);
    //                        await _context.SaveChangesAsync();

    //                    }                  

    //            }
    //        }

    //        return 1;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Error: " + ex.Message);
    //        return null;
    //    }
    //}

    public async Task<int?> SubmitAssetsAsync(UserUploadTaxAssistedDocDto asset)
    {
        try
        {
            //foreach (var asset in assets)
            //  {
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
                    content.Add(fileContent, "file", file.FileName);  // Adjust 'files' key if needed by external API
                    content.Add(new StringContent(asset.UserId.ToString()), "userId");
                    content.Add(new StringContent(DateTime.Now.Year.ToString()), "year");

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
            // }
            return null;
        }
        catch
        {
            // Optionally log the error
            return null;
        }
    }
    private UserUploadTaxAssistedDoc CloneWithNote(UserUploadTaxAssistedDoc source, string note)
    {
        return new UserUploadTaxAssistedDoc
        {
            UserId = source.UserId,
            Year = source.Year,
            UploadDate = source.UploadDate,
            CategoryName = source.CategoryName,

            UploadedFileName = source.UploadedFileName,
            FileName = source.FileName,
            Location = source.Location,
            DecryptionKey = source.DecryptionKey,
            UploadId = source.UploadId,
            OriginalName = source.OriginalName,
            UploadTime = source.UploadTime,

            AssestOptionType = source.AssestOptionType,
            AssetCategory = source.AssetCategory,
            AssetVehicleType = source.AssetVehicleType,
            AssetInstitution = source.AssetInstitution,
            AssestType = source.AssestType,

            AssetNote = note
        };
    }

    public async Task<bool> DeleteAllUploadedDocsByUserAndYear(string userId, int assessmentYear)
    {
        var docs = await _context.UserUploadTaxAssistedDocs
            .Where(d => d.UserId == userId && d.Year == assessmentYear)
            .ToListAsync();

        if (!docs.Any())
            return false;

        _context.UserUploadTaxAssistedDocs.RemoveRange(docs);
        await _context.SaveChangesAsync();

        return true;
    }
}
