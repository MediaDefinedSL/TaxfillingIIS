using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Interfaces
{
    public interface IUserTaxAssistedOtherAssetsDetailsRepository
    {
        Task SaveUserOtherTaxDetailsAsync(UserTaxAssistedOtherAssetsDetailsDto dto);
        Task<UserTaxAssistedOtherAssetsDetailsDto> GetByUserAndYearAsync(string userId, string assessmentYear);

    }
}
