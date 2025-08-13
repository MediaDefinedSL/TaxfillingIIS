using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaxFiling.Business.Interfaces;
using TaxFiling.Domain.Auth;
using TaxFiling.Domain.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxFiling.API.Services;
using TaxFiling.Domain.Common;
using Microsoft.AspNetCore.Identity;
using TaxFiling.Domain.Entities;
using TaxFiling.Data;
using TaxFiling.Business.Repositories;
using System.Xml.Linq;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace TaxFiling.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserUploadTaxAssistedDocController : ControllerBase
{
    private readonly IUserUploadTaxAssistedDocRepository _userUploadTaxAssistedDocRepository;
    private readonly IConfiguration _configuration;

    public UserUploadTaxAssistedDocController(IUserUploadTaxAssistedDocRepository userUploadTaxAssistedDocRepository, IConfiguration configuration)
    {
        _userUploadTaxAssistedDocRepository = userUploadTaxAssistedDocRepository;
        _configuration = configuration;


    }
    [HttpGet("uploaduser_list")]
    public async Task<IActionResult> GetUploadUserList(CancellationToken ctx)
    {
        try
        {
            var uploadUserList = await _userUploadTaxAssistedDocRepository.GetUploadUserList(ctx);
           
            return Ok(uploadUserList);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("SaveUploadedDocs")]
    public async Task<IActionResult> SaveUploadedDocs([FromBody] UserUploadTaxAssistedDocDto dto)
    {
        var userUploadId = await _userUploadTaxAssistedDocRepository.SaveUploadedDocsContent(dto);
        if (userUploadId.HasValue)
        {
            return Ok(new
            {
                message = "Saved successfully",
                userUploadId = userUploadId.Value
            });
        }
        else
        {
            return StatusCode(500, "Failed to save data");
        }
    }

    [HttpGet("GetUploadedDocsByUser")]
    public async Task<IActionResult> GetUploadedDocsByUser(string userId)
    {
        var docs = await _userUploadTaxAssistedDocRepository.GetUploadedDocsByUser(userId);
        return Ok(docs);
    }

    [HttpDelete("DeleteDoc/{id}")]
    public async Task<IActionResult> DeleteDoc(int id)
    {
        var result = await _userUploadTaxAssistedDocRepository.DeleteUploadedDocAsync(id);
        if (result)
            return Ok(new { message = "Document deleted successfully" });
        else
            return NotFound(new { message = "Document not found or could not be deleted" });
    }

    [HttpPost("submitassets")]
    public async Task<IActionResult> SubmitAssets([FromForm] UserUploadTaxAssistedDocDto asset)
    {
        var result = await _userUploadTaxAssistedDocRepository.SubmitAssetsAsync(asset);

        return Ok(new { message = "Form data received successfully" });
    }

    

}

    

