using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Repositories;

public class SelfOnlineFlowRepository : ISelfOnlineFlowRepository
{
    private readonly Context _context;
    private readonly ILogger<PackagesRepository> _logger;
    private readonly IConfiguration _configuration;
    public SelfOnlineFlowRepository(Context context, ILogger<PackagesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TaxPayerDetailsDto>> GetTaxPayers(string userId, int year, CancellationToken ctx)
    {
        try
        {
            var taxPayers = await (
             from t in _context.TaxPayers
             join s in _context.SelfOnlineFlowPersonalInformation
                 on new { TaxpayerId = t.Id, UserId = userId, Year = year }
                 equals new { TaxpayerId = s.TaxpayerId ?? 0, s.UserId, s.Year }
                 into ps
                 from s in ps.DefaultIfEmpty()
                 select new TaxPayerDetailsDto
                 {
                     UserId = s != null ? s.UserId : userId,
                     Year = s != null ? s.Year : year,
                     TaxpayerId = t.Id,
                     Name = t.Name,
                     ImageUrl = t.ImageUrl,
                     SpouseName = s != null ? s.SpouseName : null,
                     SpouseTINNo = s != null ? s.SpouseTINNo : null,
                     SpouseNIC = s != null ? s.SpouseNIC : null,
                     SomeoneName = s != null ? s.SomeoneName : null,
                     Relationship = s != null ? s.Relationship : null,
                     SomeoneTINNo = s != null ? s.SomeoneTINNo : null
                 }
             ).ToListAsync(ctx);

            return taxPayers;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new List<TaxPayerDetailsDto>();
        }
    }

    public async Task<List<MaritalStatusDto>> GetMaritalStatus(CancellationToken cancellationToken)
    {
        List<MaritalStatusDto> maritalStatus = [];
        try
        {

            maritalStatus = await _context.MaritalStatuses
                .Select(t => new MaritalStatusDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    ImageUrl = t.ImageUrl

                }).ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return maritalStatus;
    }

    public async Task<List<TaxReturnLastyearDto>> GetTaxReturnLastyears(CancellationToken cancellationToken)
    {
        List<TaxReturnLastyearDto> taxReturnLastyears = [];
        try
        {

            taxReturnLastyears = await _context.TaxReturnLastyears
                .Select(t => new TaxReturnLastyearDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    ImageUrl = t.ImageUrl

                }).ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return taxReturnLastyears;
    }

    public async Task<bool> SaveUserIdYear(string userId, int year)
    {
        bool isSuccess = false;


        try
        {
            await _context.SelfOnlineFlowPersonalInformation.AddAsync(new SelfOnlineFlowPersonalInformation
            {
                UserId = userId,
                Year = year
            });
            await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }
        return isSuccess;
    }

    public async Task<SelfOnlineFlowPersonalInformationDto> GetSelfOnlineFlowPersonalInformationDetails(string userId, int year, CancellationToken ctx)
    {

        var bookingViewModel = await _context.SelfOnlineFlowPersonalInformation.AsNoTracking()
                                        .Where(b => b.UserId == userId && b.Year == year)
                                        .Select(b => new SelfOnlineFlowPersonalInformationDto
                                        {
                                            UserId = userId,
                                            Year = year,
                                            TaxpayerId = b.TaxpayerId,
                                            MaritalStatusId = b.MaritalStatusId,
                                            TaxReturnLastYearId = b.TaxReturnLastYearId,
                                            FirstName = b.FirstName,
                                            MiddleName = b.MiddleName,
                                            LastName = b.LastName,
                                            DateOfBirth = b.DateOfBirth,
                                            TaxNumber = b.TaxNumber,
                                            NIC_NO=b.NIC_NO,
                                            Address = b.Address,
                                            Gender = b.Gender,
                                            CareOf = b.CareOf,
                                            Apt = b.Apt,
                                            StreetNumber = b.StreetNumber,
                                            Street = b.Street,
                                            City = b.City,
                                            SomeoneName = b.SomeoneName,
                                            SomeoneTINNo = b.SomeoneTINNo,
                                            SpouseNIC = b.SpouseNIC,
                                            SpouseName = b.SpouseName,
                                            SpouseTINNo = b.SpouseTINNo,
                                            Relationship = b.Relationship

                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(ctx);
        return bookingViewModel;

    }

    public async Task<bool> UpdateTaxPayer(TaxPayerDetailsDto taxPayerdetails)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == taxPayerdetails.UserId && p.Year == taxPayerdetails.Year)
                                .FirstOrDefaultAsync();


            if(taxPayerdetails.TaxpayerId == 2)
            {
                taxPayerdetails.SomeoneName = null;
                taxPayerdetails.Relationship = null;
                taxPayerdetails.SomeoneTINNo = null;
            }
            else if (taxPayerdetails.TaxpayerId == 3)
            {
                taxPayerdetails.SpouseName = null;
                taxPayerdetails.SpouseTINNo = null;
                taxPayerdetails.SpouseNIC = null;
            }
            else
            {
                taxPayerdetails.SpouseName = null;
                taxPayerdetails.SpouseTINNo = null;
                taxPayerdetails.SpouseNIC = null;
                taxPayerdetails.SomeoneName = null;
                taxPayerdetails.Relationship = null;
                taxPayerdetails.SomeoneTINNo = null;
            }

            _user.TaxpayerId = taxPayerdetails.TaxpayerId;
            _user.SpouseName = taxPayerdetails.SpouseName;
            _user.SpouseTINNo = taxPayerdetails.SpouseTINNo;
            _user.SpouseNIC = taxPayerdetails.SpouseNIC;
            _user.SomeoneName = taxPayerdetails.SomeoneName;
            _user.Relationship = taxPayerdetails.Relationship;
            _user.SomeoneTINNo = taxPayerdetails.SomeoneTINNo;

            await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update TaxPayerId");
        }
        return isSuccess;
    }

    public async Task<bool> UpdateMaritalStatus(string userId, int year, int maritalStatusId)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == userId && p.Year == year)
                                .FirstOrDefaultAsync();

            _user.MaritalStatusId = maritalStatusId;

            await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update MaritalStatus");
        }
        return isSuccess;
    }

    public async Task<bool> UpdatelLastYear(string userId, int year, int lastyearId)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == userId && p.Year == year)
                                .FirstOrDefaultAsync();

            _user.TaxReturnLastYearId = lastyearId;

            await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<bool> UpdatelIdentification(IdentificationsDto identifications)
    {
        bool isSuccess = false;


        try
        {
            var _selfOnlineuser = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == identifications.UserId && p.Year == identifications.Year)
                                .FirstOrDefaultAsync();

            if (_selfOnlineuser != null)
            {
                _selfOnlineuser.FirstName = identifications.FirstName;
                _selfOnlineuser.MiddleName = identifications.MiddleName;
                _selfOnlineuser.LastName = identifications.LastName;
                _selfOnlineuser.DateOfBirth = identifications.DateOfBirth;
                _selfOnlineuser.TaxNumber = identifications.TaxNumber;
                _selfOnlineuser.NIC_NO = identifications.NIC_NO;
                _selfOnlineuser.Address = identifications.Address;
                _selfOnlineuser.Gender = identifications.Gender;

                await _context.SaveChangesAsync();
            }

            var _user = await _context.Users
                .Where(p => p.UserId.ToString() == identifications.UserId)
                .FirstOrDefaultAsync();

            if (_user != null)
            {
                _user.FirstName = identifications.FirstName;
                _user.LastName = identifications.LastName;
               // _user.TinNo = identifications.TaxNumber;

                await _context.SaveChangesAsync();
            }
            isSuccess = true;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<bool> UpdatelContactInformation(string userId, int year, string careof, string apt, string streetnumber, string street, string city)
    {
        bool isSuccess = false;

        try
        {
            var _selfOnlineuser = await _context.SelfOnlineFlowPersonalInformation
                               .Where(p => p.UserId == userId && p.Year == year)
                               .FirstOrDefaultAsync();

            if (_selfOnlineuser != null)
            {
                _selfOnlineuser.CareOf = careof;
                _selfOnlineuser.Apt = apt;
                _selfOnlineuser.StreetNumber = streetnumber;
                _selfOnlineuser.Street = street;
                _selfOnlineuser.City = city;

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<bool> SaveSelfOnlineEmploymentIncome(SelfOnlineEmploymentIncomeDto selfOnlineEmploymentIncome)
    {
        bool isSuccess = false;

        var dbTrans = await _context.Database.BeginTransactionAsync();
        try
        {


            var _employmentIncomuser = await _context.SelfOnlineEmploymentIncomes
                               .Where(p => p.UserId == selfOnlineEmploymentIncome.UserId && p.Year == selfOnlineEmploymentIncome.Year)
                               .FirstOrDefaultAsync();

            if (_employmentIncomuser == null)
            {
                var _employmentIncome = new SelfOnlineEmploymentIncome
                {
                    UserId = selfOnlineEmploymentIncome.UserId,
                    Year= selfOnlineEmploymentIncome.Year,
                    Residency = selfOnlineEmploymentIncome.Residency,
                    SeniorCitizen = selfOnlineEmploymentIncome.SeniorCitizen,
                    TerminalBenefits = selfOnlineEmploymentIncome.TerminalBenefits,
                    ExemptAmounts = selfOnlineEmploymentIncome.ExemptAmounts,
                    CreatedBy = selfOnlineEmploymentIncome.UserId,
                    CreatedOn = DateTime.Now

                };

                _context.SelfOnlineEmploymentIncomes.Add(_employmentIncome);
                await _context.SaveChangesAsync();

            }
            else {
                _employmentIncomuser.Residency = selfOnlineEmploymentIncome.Residency;
                _employmentIncomuser.SeniorCitizen = selfOnlineEmploymentIncome.SeniorCitizen;
                _employmentIncomuser.UpdatedBy = selfOnlineEmploymentIncome.UserId;
                _employmentIncomuser.UpdatedOn = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            isSuccess = true;


            await dbTrans.CommitAsync();

            //Message = "Successfully registered the User.";
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }
    public async Task<SelfOnlineEmploymentIncomeDto> GetSelfOnlineEmploymentIncome(string userId, int year, CancellationToken ctx)
    {

        var EmploymentIncomeModel = await _context.SelfOnlineEmploymentIncomes.AsNoTracking()
                                        .Where(b => b.UserId == userId && b.Year == year)
                                        .Select(b => new SelfOnlineEmploymentIncomeDto
                                        {
                                            SelfOnlineEmploymentIncomeId = b.SelfOnlineEmploymentIncomeId,
                                            UserId = userId,
                                            Year = year,
                                            SeniorCitizen=b.SeniorCitizen,
                                            Residency = b.Residency,
                                            TerminalBenefits = b.TerminalBenefits,
                                            ExemptAmounts = b.ExemptAmounts,
                                            Total = b.Total

                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(ctx);
        return EmploymentIncomeModel;

    }

    public async Task<bool> SaveSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails)
    {
        bool isSuccess = false;

        var dbTrans = await _context.Database.BeginTransactionAsync();
        try
        {
            decimal? addToTotal = 0;

            if (selfOnlineEmploymentIncomeDetails.CategoryName == "EmploymentDetails")
            {
                var _employmentDetailsIncome = new SelfOnlineEmploymentIncomeDetails
                {
                    SelfOnlineEmploymentIncomeId = selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentIncomeId,
                    UserId = selfOnlineEmploymentIncomeDetails.UserId,
                    Year=selfOnlineEmploymentIncomeDetails.Year,
                    CategoryName = selfOnlineEmploymentIncomeDetails.CategoryName,
                    TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName,
                    EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName,
                    TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer,
                    Remuneration = selfOnlineEmploymentIncomeDetails.Remuneration,
                    APITPrimaryEmployment = selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment,
                    APITSecondaryEmployment = selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment,
                    CreatedBy = selfOnlineEmploymentIncomeDetails.UserId,
                    CreatedOn = DateTime.Now

                };

                _context.SelfOnlineEmploymentIncomeDetails.Add(_employmentDetailsIncome);
                await _context.SaveChangesAsync();


                addToTotal = (selfOnlineEmploymentIncomeDetails.Remuneration ?? 0)
                       + (selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment ?? 0)
                       + (selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment ?? 0);

               

            }
            else if (selfOnlineEmploymentIncomeDetails.CategoryName == "TerminalBenefits")
            {
                var _terminalBenefits = new SelfOnlineEmploymentIncomeDetails
                {
                    SelfOnlineEmploymentIncomeId = selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentIncomeId,
                    UserId = selfOnlineEmploymentIncomeDetails.UserId,
                    Year = selfOnlineEmploymentIncomeDetails.Year,
                    CategoryName = selfOnlineEmploymentIncomeDetails.CategoryName,
                    TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName,
                    EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName,
                    TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer,
                    TerminalBenefits = selfOnlineEmploymentIncomeDetails.TerminalBenefits,
                    CreatedBy = selfOnlineEmploymentIncomeDetails.UserId,
                    CreatedOn = DateTime.Now

                };

                _context.SelfOnlineEmploymentIncomeDetails.Add(_terminalBenefits);
                await _context.SaveChangesAsync();

                addToTotal = (selfOnlineEmploymentIncomeDetails.TerminalBenefits ?? 0);
            }
            else if (selfOnlineEmploymentIncomeDetails.CategoryName == "ExemptAmounts")
            {
                var _exemptAmounts = new SelfOnlineEmploymentIncomeDetails
                {
                    SelfOnlineEmploymentIncomeId = selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentIncomeId,
                    UserId = selfOnlineEmploymentIncomeDetails.UserId,
                    Year = selfOnlineEmploymentIncomeDetails.Year,
                    CategoryName = selfOnlineEmploymentIncomeDetails.CategoryName,
                    TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName,
                    EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName,
                    TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer,
                    Amount = selfOnlineEmploymentIncomeDetails.Amount,
                    CreatedBy = selfOnlineEmploymentIncomeDetails.UserId,
                    CreatedOn = DateTime.Now

                };

                _context.SelfOnlineEmploymentIncomeDetails.Add(_exemptAmounts);
                await _context.SaveChangesAsync();

                addToTotal = (selfOnlineEmploymentIncomeDetails.Amount ?? 0);
            }

            var mainIncome = await _context.SelfOnlineEmploymentIncomes
           .FirstOrDefaultAsync(x => x.SelfOnlineEmploymentIncomeId == selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentIncomeId);

            var mainTaxIncome = await _context.Users
           .FirstOrDefaultAsync(x => x.UserId.ToString() == selfOnlineEmploymentIncomeDetails.UserId  );

            if (mainIncome != null)
            {
                mainIncome.Total = (mainIncome.Total ?? 0) + addToTotal;
                _context.SelfOnlineEmploymentIncomes.Update(mainIncome);
                await _context.SaveChangesAsync();
            }
            if (mainTaxIncome != null)
            {
                mainTaxIncome.TaxTotal = (mainTaxIncome.TaxTotal ?? 0) + addToTotal;
                _context.Users.Update(mainTaxIncome);
                await _context.SaveChangesAsync();
            }

            isSuccess = true;

            await dbTrans.CommitAsync();

            //Message = "Successfully registered the User.";
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<List<SelfOnlineEmploymentIncomeDetailsDto>> GetSelfOnlineEmploymentIncomeList(string userId, int year, CancellationToken ctx)
    {
        List<SelfOnlineEmploymentIncomeDetailsDto> employmentIncome = [];
        try
        {
            
                employmentIncome = await _context.SelfOnlineEmploymentIncomeDetails
                .Where(b => b.UserId == userId && b.Year == year)
                .Select(t => new SelfOnlineEmploymentIncomeDetailsDto
                {
                    SelfOnlineEmploymentDetailsId = t.SelfOnlineEmploymentDetailsId,
                    SelfOnlineEmploymentIncomeId = t.SelfOnlineEmploymentIncomeId,
                    CategoryName = t.CategoryName,
                    TypeOfName = t.TypeOfName,
                    EmployerORCompanyName = t.EmployerORCompanyName,
                    TINOfEmployer = t.EmployerORCompanyName,
                    Remuneration = t.Remuneration,
                    APITPrimaryEmployment = t.APITPrimaryEmployment,
                    APITSecondaryEmployment = t.APITSecondaryEmployment,
                    TerminalBenefits = t.TerminalBenefits,
                    Amount = t.Amount

                }).ToListAsync(ctx);
          

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return employmentIncome;
    }

    public async Task<bool> UpdateEmploymentIncomeTerminalBenefits(string userId, int year, int employmentIncomeId, bool terminalBenefits )
    {
        bool isSuccess = false;


        try
        {
            var _employmentIncomuser = await _context.SelfOnlineEmploymentIncomes
                              .Where(p => p.UserId == userId && p.Year == year && p.SelfOnlineEmploymentIncomeId == employmentIncomeId)
                              .FirstOrDefaultAsync();

            if (_employmentIncomuser != null)
            {
                _employmentIncomuser.TerminalBenefits = terminalBenefits;
            }

          

            if (!terminalBenefits)
            {
                var recordsToDelete = await _context.SelfOnlineEmploymentIncomeDetails
                    .Where(b => b.UserId == userId
                             && b.Year == year
                             && b.SelfOnlineEmploymentIncomeId == employmentIncomeId
                             && b.CategoryName == "TerminalBenefits")
                    .ToListAsync();

                if (recordsToDelete.Any())
                {
                    _context.SelfOnlineEmploymentIncomeDetails.RemoveRange(recordsToDelete);
                }
            }
            await _context.SaveChangesAsync();
            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update MaritalStatus");
        }
        return isSuccess;
    }

    public async Task<bool> UpdateEmploymentIncomeExemptAmounts(string userId, int year, int employmentIncomeId, bool exemptAmounts)
    {
        bool isSuccess = false;


        try
        {
            var _employmentIncomuser = await _context.SelfOnlineEmploymentIncomes
                              .Where(p => p.UserId == userId && p.Year == year && p.SelfOnlineEmploymentIncomeId == employmentIncomeId)
                              .FirstOrDefaultAsync();
            if (_employmentIncomuser != null)
            {
                _employmentIncomuser.ExemptAmounts = exemptAmounts;
            }

            if (!exemptAmounts)
            {
                var recordsToDelete = await _context.SelfOnlineEmploymentIncomeDetails
                    .Where(b => b.UserId == userId
                             && b.Year == year
                             && b.SelfOnlineEmploymentIncomeId == employmentIncomeId
                             && b.CategoryName == "ExemptAmounts")
                    .ToListAsync();

                if (recordsToDelete.Any())
                {
                    _context.SelfOnlineEmploymentIncomeDetails.RemoveRange(recordsToDelete);
                }
            }

            await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update MaritalStatus");
        }
        return isSuccess;
    }

}


