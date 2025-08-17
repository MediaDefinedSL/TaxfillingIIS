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



namespace TaxFiling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserTaxAssistedOtherAssetsDetailsController : ControllerBase
    {
        private readonly IUserTaxAssistedOtherAssetsDetailsRepository _userTaxAssistedOtherAssetsDetailsRepository;
        private readonly IConfiguration _configuration;

        public UserTaxAssistedOtherAssetsDetailsController(IUserTaxAssistedOtherAssetsDetailsRepository userTaxAssistedOtherAssetsDetails, IConfiguration configuration)
        {
            _userTaxAssistedOtherAssetsDetailsRepository = userTaxAssistedOtherAssetsDetails;
            _configuration = configuration;
        }

        [HttpPost("SaveUserOtherTaxDetails")]
        public async Task<IActionResult> SaveUserOtherTaxDetails([FromBody] UserTaxAssistedOtherAssetsDetailsDto dto)
        {
            await _userTaxAssistedOtherAssetsDetailsRepository.SaveUserOtherTaxDetailsAsync(dto);
            return Ok(new { message = "Other asset detail saved successfully"});
        }
        [HttpGet("GetUserOtherTaxDetails")]
        public async Task<IActionResult> GetUserOtherTaxDetails(string userId, string assessmentYear)
        {
            var data = await _userTaxAssistedOtherAssetsDetailsRepository.GetByUserAndYearAsync(userId, assessmentYear);
            //if (data == null)
            //    return NotFound();

            return Ok(data);
        }

        [HttpDelete("DeleteDraftOtherTaxByUserAndYear")]
        public async Task<IActionResult> DeleteDraftOtherTaxByUserAndYear([FromQuery] string userId, [FromQuery] string assessmentYear)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(assessmentYear))
                return BadRequest("UserId and AssessmentYear are required.");

            try
            {
                // Call service to delete draft data
                bool deleted = await _userTaxAssistedOtherAssetsDetailsRepository.DeleteDraftOtherAssetsByUserAndYear(userId, assessmentYear);

                //if (!deleted)
                    //return NotFound("No draft data found to delete.");

                return Ok("Draft Other Assets data deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
