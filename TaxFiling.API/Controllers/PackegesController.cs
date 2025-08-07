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

namespace TaxFiling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackegesController : ControllerBase
    {

        private readonly IPackagesRepository _packagesRepository;
        private readonly IConfiguration _configuration;
       

        public PackegesController(IPackagesRepository packagesRepository, IConfiguration configuration)
        {
            _packagesRepository = packagesRepository;
            _configuration = configuration;
          

        }
        [HttpGet("list")]
        public async Task<IActionResult> GetPackages(int isSelfFiling , CancellationToken ctx)
        {
            try
            {
              //  var IsSelfFiling = 1;
                var varients = await _packagesRepository.GetPackages(isSelfFiling, ctx);
                var responseResult = new ResponseResult
                {
                    Data = varients
                };

                return Ok(varients);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

       


        [HttpGet("get")]
        public async Task<IActionResult> GetPackageDetails(int id, CancellationToken ctx)
        {
            try
            {
                var result = await _packagesRepository.GetPackageDetails(id, ctx);
                if (result is null)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
