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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaxFiling.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SelfOnlineFlowController : ControllerBase
{
    private readonly ISelfOnlineFlowRepository _selfOnlineFlowRepository;
    private readonly IConfiguration _configuration;

    public SelfOnlineFlowController(ISelfOnlineFlowRepository selfOnlineFlowRepository, IConfiguration configuration)
    {
        _selfOnlineFlowRepository = selfOnlineFlowRepository;
        _configuration = configuration;


    }

    [HttpGet("taxpayer_list")]
    public async Task<IActionResult> GetTaxPayers(string userId, int year,CancellationToken ctx)
    {
        try
        {
            var taxPayers = await _selfOnlineFlowRepository.GetTaxPayers(userId, year,ctx);
            //var responseResult = new ResponseResult
            //{
            //    Data = varients
            //};

            return Ok(taxPayers);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("maritalStatus_list")]
    public async Task<IActionResult> GetMaritalStatus(CancellationToken ctx)
    {
        try
        {
            var taxPayers = await _selfOnlineFlowRepository.GetMaritalStatus(ctx);
           

            return Ok(taxPayers);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("taxlastyears_list")]
    public async Task<IActionResult> GetTaxReturnLastyears(CancellationToken ctx)
    {
        try
        {
            var taxPayers = await _selfOnlineFlowRepository.GetTaxReturnLastyears(ctx);
           ;

            return Ok(taxPayers);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("save_useridyear")]
    public async Task<IActionResult> SaveUserIdYear(string userId, int year)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.SaveUserIdYear(userId, year);
           
            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("sofpersonalinformation_details")]
    public async Task<IActionResult> GetSelfOnlineFlowPersonalInformationDetails(string userId,int year, CancellationToken ctx)
    {
        try
        {
            var personalInformationDetails = await _selfOnlineFlowRepository.GetSelfOnlineFlowPersonalInformationDetails(userId, year, ctx);
            if (personalInformationDetails is null)
            {
                return NoContent();
            }

            return Ok(personalInformationDetails);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("update_taxpayer")]
    public async Task<IActionResult> UpdateTaxPayer(TaxPayerDetailsDto taxPayerdetails)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdateTaxPayer(taxPayerdetails);
           
            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("update_maritalstatus")]
    public async Task<IActionResult> UpdateMaritalStatus(string userId, int year, int maritalStatusId)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdateMaritalStatus(userId, year, maritalStatusId);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("update_lastyear")]
    public async Task<IActionResult> UpdatelLastYear(string userId, int year, int lastyearId)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdatelLastYear(userId, year, lastyearId);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("update_identification")]
    public async Task<IActionResult> UpdatelIdentification(IdentificationsDto identifications)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdatelIdentification(identifications);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("update_contactinformation")]
    public async Task<IActionResult> UpdatelContactInformation(string userId, int year, string? careof,string? apt , string streetnumber , string street , string city)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdatelContactInformation(userId, year, careof , apt, streetnumber, street, city);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("save_employmentincome")]
    public async Task<IActionResult> SaveSelfOnlineEmploymentIncome(SelfOnlineEmploymentIncomeDto selfOnlineEmploymentIncome)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.SaveSelfOnlineEmploymentIncome(selfOnlineEmploymentIncome);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get_employmentincome")]
    public async Task<IActionResult> GetSelfOnlineEmploymentIncome(string userId, int year, CancellationToken ctx)
    {
        try
        {
            var employmentIncome = await _selfOnlineFlowRepository.GetSelfOnlineEmploymentIncome(userId, year, ctx);
            if (employmentIncome is null)
            {
                return NoContent();
            }

            return Ok(employmentIncome);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("save_employmentincomedetails")]
    public async Task<IActionResult> SaveSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.SaveSelfOnlineEmploymentIncomeDetails(selfOnlineEmploymentIncomeDetails);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("employmentincome_list")]
    public async Task<IActionResult> GetSelfOnlineEmploymentIncomeList(string userId, int year,  CancellationToken ctx)
    {
        try
        {
            var employmentincomeList = await _selfOnlineFlowRepository.GetSelfOnlineEmploymentIncomeList(userId, year,  ctx);
            //var responseResult = new ResponseResult
            //{
            //    Data = varients
            //};

            return Ok(employmentincomeList);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("update_eincometerminalbenefits")]
    public async Task<IActionResult> UpdateEmploymentIncomeTerminalBenefits(string userId, int year, int employmentIncomeId , bool terminalBenefits)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdateEmploymentIncomeTerminalBenefits(userId, year, employmentIncomeId, terminalBenefits);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update_eincomeexemptamounts")]
    public async Task<IActionResult> UpdateEmploymentIncomeExemptAmounts(string userId, int year, int employmentIncomeId, bool exemptAmounts)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdateEmploymentIncomeExemptAmounts(userId, year, employmentIncomeId, exemptAmounts);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("update_employmentincomedetails")]
    public async Task<IActionResult> UpdateSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails)
    {
        try
        {
            var isSuccess = await _selfOnlineFlowRepository.UpdateSelfOnlineEmploymentIncomeDetails(selfOnlineEmploymentIncomeDetails);

            return Ok(isSuccess);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


}
