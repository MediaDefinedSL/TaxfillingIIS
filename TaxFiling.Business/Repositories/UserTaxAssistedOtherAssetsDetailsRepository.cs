using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;


namespace TaxFiling.Business.Repositories
{
    public class UserTaxAssistedOtherAssetsDetailsRepository : IUserTaxAssistedOtherAssetsDetailsRepository
    {
        private readonly Context _context;
        private readonly ILogger<PackagesRepository> _logger;
        private readonly IConfiguration _configuration;


        public UserTaxAssistedOtherAssetsDetailsRepository(Context context, ILogger<PackagesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task SaveUserOtherTaxDetailsAsync(UserTaxAssistedOtherAssetsDetailsDto dto)
        {
            var existingRecords = _context.UserTaxAssistedOtherAssetsDetails
        .Where(x => x.UserId == dto.UserId && x.AssessmentYear == dto.AssessmentYear);

            _context.UserTaxAssistedOtherAssetsDetails.RemoveRange(existingRecords);
            await _context.SaveChangesAsync();


            foreach (var detail in dto.Details)
            {
                var entity = new UserTaxAssistedOtherAssetsDetails
                {
                    UserId = dto.UserId,
                    OtherAssetCategory = detail.Category,
                    OtherAssetValue = decimal.TryParse(detail.Value, out var val) ? val : 0,
                    AssessmentYear = dto.AssessmentYear,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                _context.UserTaxAssistedOtherAssetsDetails.Add(entity);
            }
            await _context.SaveChangesAsync();

            
        }

        public async Task<UserTaxAssistedOtherAssetsDetailsDto> GetByUserAndYearAsync(string userId, string assessmentYear)
        {
            var records = await _context.UserTaxAssistedOtherAssetsDetails
                .Where(x => x.UserId == userId && x.AssessmentYear == assessmentYear)
                .ToListAsync();

            if (!records.Any())
                return null;

            var dto = new UserTaxAssistedOtherAssetsDetailsDto
            {
                UserId = userId,
                AssessmentYear = assessmentYear,                
                Details = records.Select(r => new OtherTaxDetailDto
                {
                    Category = r.OtherAssetCategory,
                    Value = r.OtherAssetValue.ToString()  ,
                    CreatedDate = r.CreatedDate.ToString("yyyy-MM-dd")
                }).ToList()
            };

            return dto;
        }

        public async Task<bool> DeleteDraftOtherAssetsByUserAndYear(string userId, string assessmentYear)
        {
            var draftRecords = await _context.UserTaxAssistedOtherAssetsDetails
                .Where(d => d.UserId == userId && d.AssessmentYear == assessmentYear)
                .ToListAsync();

            if (!draftRecords.Any())
                return false;

            _context.UserTaxAssistedOtherAssetsDetails.RemoveRange(draftRecords);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
