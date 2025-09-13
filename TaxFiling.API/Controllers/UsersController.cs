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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;



namespace TaxFiling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
   

        public UsersController(IUserRepository userRepository, IConfiguration configuration, UserService userService) {
            _userRepository = userRepository;
            _configuration = configuration;
            _userService = userService;
          
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(CancellationToken cancellationToken) {
            try {
                var users = await _userRepository.GetUsers(cancellationToken);
                return Ok(users);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Get(UserDto mUser)
        {
            try
            {
                var user = await _userRepository.GetUser(mUser);
                if (user is null)
                {
                    return BadRequest("User not found");
                }

                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserCode", user.UserId.ToString()),
                    new Claim("UserName", user.UserName),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(1),
                        signingCredentials: signIn
                    );

                var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                var jwtToken = new TokenModel
                {
                    AccessToken = tokenValue
                };

                return Ok(jwtToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            try
            {
                // var invokeBy = _userService.GetUserCode();

                var isSuccess = await _userRepository.AddUser(user);
                var responseResult = new ResponseResult
                {
                    Success = isSuccess,
                    Message = _userRepository.Message,
                    ResultGuid = _userRepository.ResultGuid,
                    Name = _userRepository.Name,
                    Data = _userRepository.Data
                };
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken ctx)
        {
            try
            {
                var result = await _userRepository.GetUser(id, ctx);
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserDto user)
        {
            try
            {
                var isSuccess = await _userRepository.UpdateUser(user);
                var responseResult = new ResponseResult
                {
                    Success = isSuccess,
                    Message = _userRepository.Message,
                    ResultGuid = _userRepository.ResultGuid
                };
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updatetinstatus")]
        public async Task<IActionResult> UpdateUserTinStatus(UserTinStatusDto userTinStatus)
        {
            try
            {
                var isSuccess = await _userRepository.UpdateUserTinStatus(userTinStatus.UserId, userTinStatus.TinStatus);
                var responseResult = new ResponseResult
                {
                    Success = isSuccess,
                    Message = _userRepository.Message,
                    ResultGuid = _userRepository.ResultGuid
                };
                return Ok(responseResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update_profileimage")]
        public async Task<IActionResult> updateProfileImage(string userId, string profileImagePath)
        {
            try
            {
                var isSuccess = await _userRepository.updateProfileImage(userId, profileImagePath);

                return Ok(isSuccess);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-document-status")]
        public async Task<IActionResult> UpdateUserUploadedDocumentStatus(UserUploadedDocumentStatusDto userDocStatus)
        {
            if (userDocStatus.UserId == Guid.Empty)
                return BadRequest("Invalid userId.");

            var result = await _userRepository.UpdateUserUploadedDocumentStatus(userDocStatus.UserId, userDocStatus.UploadedDocumentStatus.Value);

            if (result)
                return Ok(new { success = true, message = "Status updated successfully." });
            else
                return BadRequest(new { success = false, message = "Failed to update status." });
        }

        [HttpGet("GetUploadedDocStatus")]
        public async Task<IActionResult> GetUploadedDocumentStatus([FromQuery] Guid userId)
        {
            var latestStatus = await _userRepository.GetLatestUploadedDocumentStatusAsync(userId);
            return Ok(latestStatus);
        }

        [HttpGet("GetPersonalInformationCompleted")]
        public async Task<IActionResult> GetPersonalInformationCompleted([FromQuery] Guid userId)
        {
            var personalInfoStatus = await _userRepository.GetPersonalInformationCompleted(userId);
            return Ok(personalInfoStatus);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] TaxFiling.Domain.Dtos.ResetPasswordRequest request)
        {

            // Update password using only email + new password
            var success = await _userRepository.UpdatePasswordAsync(request.Email, request.NewPassword);

            if (!success)
                return StatusCode(500, "Failed to reset password.");

            return Ok("Password reset successful.");
        }


    }
}
