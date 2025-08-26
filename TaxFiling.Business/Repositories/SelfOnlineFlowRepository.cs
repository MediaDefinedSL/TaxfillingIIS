using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
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
                                            NIC_NO = b.NIC_NO,
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


            if (taxPayerdetails.TaxpayerId == 2)
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

    public async Task<bool> UpdatelContactInformation(string userId, int year, string? careof, string? apt, string streetnumber, string street, string city)
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
                    Year = selfOnlineEmploymentIncome.Year,
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
            else
            {
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

                                            UserId = userId,
                                            Year = year,
                                            SeniorCitizen = b.SeniorCitizen,
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

        //  var dbTrans = await _context.Database.BeginTransactionAsync();
        try
        {


            await _context.Database.ExecuteSqlRawAsync(
            @"CALL ADDEditSelfOnlineEmploymentIncomeDetails(
                @loguser,
                @UserId,
                @Year,
                @MainCategoryName,
                @CategoryName,
                @TransactionType,
                @SelfOnlineEmploymentDetailsId,
                @Residency,
                @SeniorCitizen,
                @TypeOfName,
                @EmployerORCompanyName,
                @TINOfEmployer,
                @Remuneration,
                @APITPrimaryEmployment,
                @APITSecondaryEmployment,
                @TerminalBenefits,
                @Amount
            )",
            new MySqlParameter("@loguser", selfOnlineEmploymentIncomeDetails.UserId ?? (object)DBNull.Value),
            new MySqlParameter("@UserId", selfOnlineEmploymentIncomeDetails.UserId ?? (object)DBNull.Value),
            new MySqlParameter("@Year", selfOnlineEmploymentIncomeDetails.Year),
            new MySqlParameter("@MainCategoryName", "EmploymentIncome"),
            new MySqlParameter("@CategoryName", selfOnlineEmploymentIncomeDetails.CategoryName ?? (object)DBNull.Value),
            new MySqlParameter("@TransactionType", "Add" ?? (object)DBNull.Value),
            new MySqlParameter("@SelfOnlineEmploymentDetailsId", 0),
            new MySqlParameter("@Residency", selfOnlineEmploymentIncomeDetails.Residency),
            new MySqlParameter("@SeniorCitizen", selfOnlineEmploymentIncomeDetails.SeniorCitizen),
            new MySqlParameter("@TypeOfName", selfOnlineEmploymentIncomeDetails.TypeOfName ?? (object)DBNull.Value),
            new MySqlParameter("@EmployerORCompanyName", selfOnlineEmploymentIncomeDetails.EmployerORCompanyName ?? (object)DBNull.Value),
            new MySqlParameter("@TINOfEmployer", selfOnlineEmploymentIncomeDetails.TINOfEmployer ?? (object)DBNull.Value),
            new MySqlParameter("@Remuneration", selfOnlineEmploymentIncomeDetails.Remuneration ?? (object)DBNull.Value),
            new MySqlParameter("@APITPrimaryEmployment", selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment ?? (object)DBNull.Value),
            new MySqlParameter("@APITSecondaryEmployment", selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment ?? (object)DBNull.Value),
            new MySqlParameter("@TerminalBenefits", selfOnlineEmploymentIncomeDetails.TerminalBenefits ?? (object)DBNull.Value),
            new MySqlParameter("@Amount", selfOnlineEmploymentIncomeDetails.Amount ?? (object)DBNull.Value)
        );




            /* decimal? addToTotal = 0;

             if (selfOnlineEmploymentIncomeDetails.CategoryName == "EmploymentDetails")
             {


                 var _employmentDetailsIncome = new SelfOnlineEmploymentIncomeDetails
                 {

                     UserId = selfOnlineEmploymentIncomeDetails.UserId,
                     Year = selfOnlineEmploymentIncomeDetails.Year,
                     CategoryName = selfOnlineEmploymentIncomeDetails.CategoryName,
                     Residency = selfOnlineEmploymentIncomeDetails.Residency,
                     SeniorCitizen = selfOnlineEmploymentIncomeDetails.SeniorCitizen,
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

                 //await _context.Database.ExecuteSqlRawAsync(
                 //         "CALL UpdateSelfFilingTotalCalculation({0}, {1}, {2},{3},{4},{5},{6})",
                 //         selfOnlineEmploymentIncomeDetails.UserId,
                 //         selfOnlineEmploymentIncomeDetails.UserId,
                 //         selfOnlineEmploymentIncomeDetails.Year,
                 //         "EmploymentIncome",
                 //         "EmploymentDetails",
                 //         "add",
                 //         addToTotal

                 //     );

             }
             else if (selfOnlineEmploymentIncomeDetails.CategoryName == "TerminalBenefits")
             {
                 var _terminalBenefits = new SelfOnlineEmploymentIncomeDetails
                 {

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

             // var mainIncome = await _context.SelfOnlineEmploymentIncomes
             //.FirstOrDefaultAsync(x => x.SelfOnlineEmploymentIncomeId == selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentIncomeId);

             var mainTaxIncome = await _context.Users
            .FirstOrDefaultAsync(x => x.UserId.ToString() == selfOnlineEmploymentIncomeDetails.UserId);

             //if (mainIncome != null)
             //{
             //    mainIncome.Total = (mainIncome.Total ?? 0) + addToTotal;
             //    _context.SelfOnlineEmploymentIncomes.Update(mainIncome);
             //    await _context.SaveChangesAsync();
             //}
             if (mainTaxIncome != null)
             {
                 mainTaxIncome.TaxTotal = (mainTaxIncome.TaxTotal ?? 0) + addToTotal;
                 _context.Users.Update(mainTaxIncome);
                 await _context.SaveChangesAsync();
             }*/

            isSuccess = true;

            //  await dbTrans.CommitAsync();

            // Message = "Successfully registered the User.";
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
                // SelfOnlineEmploymentIncomeId = t.SelfOnlineEmploymentIncomeId,
                Residency = t.Residency,
                SeniorCitizen = t.SeniorCitizen,
                CategoryName = t.CategoryName,
                TypeOfName = t.TypeOfName,
                EmployerORCompanyName = t.EmployerORCompanyName,
                TINOfEmployer = t.TINOfEmployer,
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

    public async Task<bool> UpdateEmploymentIncomeTerminalBenefits(string userId, int year, int employmentIncomeId, bool terminalBenefits)
    {
        bool isSuccess = false;


        try
        {
            var employmentIncome = await _context.SelfOnlineEmploymentIncomes
                              .Where(p => p.UserId == userId && p.Year == year)
                              .FirstOrDefaultAsync();
            if (employmentIncome != null)
            {
                employmentIncome.TerminalBenefits = terminalBenefits;

                if (!terminalBenefits)
                {
                    // Find all terminal benefits records
                    var recordsToDelete = await _context.SelfOnlineEmploymentIncomeDetails
                        .Where(b => b.UserId == userId
                                 && b.Year == year
                                 // && b.SelfOnlineEmploymentIncomeId == employmentIncomeId
                                 && b.CategoryName == "TerminalBenefits")
                        .ToListAsync();

                    if (recordsToDelete.Any())
                    {
                        // Calculate the sum of TerminalBenefits before deleting
                        var sumTerminalBenefits = recordsToDelete
                            .Where(r => r.TerminalBenefits.HasValue)
                            .Sum(r => r.TerminalBenefits.Value);

                        // Subtract from parent Total
                        if (employmentIncome.Total.HasValue)
                        {
                            employmentIncome.Total = (employmentIncome.Total ?? 0) - sumTerminalBenefits;

                            var mainTaxIncome = await _context.Users
                            .FirstOrDefaultAsync(x => x.UserId.ToString() == userId);


                            if (mainTaxIncome != null)
                            {
                                mainTaxIncome.TaxTotal = (mainTaxIncome.TaxTotal ?? 0) - sumTerminalBenefits;
                                _context.Users.Update(mainTaxIncome);
                                // await _context.SaveChangesAsync();
                            }
                        }
                        // Remove records
                        _context.SelfOnlineEmploymentIncomeDetails.RemoveRange(recordsToDelete);
                    }
                }

                await _context.SaveChangesAsync();
                isSuccess = true;
            }

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
                              .Where(p => p.UserId == userId && p.Year == year)
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

                             && b.CategoryName == "ExemptAmounts")
                    .ToListAsync();

                if (recordsToDelete.Any())
                {
                    // Calculate the sum of TerminalBenefits before deleting
                    var sumExemptAmounts = recordsToDelete
                        .Where(r => r.Amount.HasValue)
                        .Sum(r => r.Amount.Value);

                    // Subtract from parent Total
                    if (_employmentIncomuser.Total.HasValue)
                    {
                        _employmentIncomuser.Total = (_employmentIncomuser.Total ?? 0) - sumExemptAmounts;

                        var mainTaxIncome = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId.ToString() == userId);


                        if (mainTaxIncome != null)
                        {
                            mainTaxIncome.TaxTotal = (mainTaxIncome.TaxTotal ?? 0) - sumExemptAmounts;
                            _context.Users.Update(mainTaxIncome);
                            // await _context.SaveChangesAsync();
                        }
                    }


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

    public async Task<bool> UpdateSelfOnlineEmploymentIncomeDetails(SelfOnlineEmploymentIncomeDetailsDto selfOnlineEmploymentIncomeDetails)
    {
        bool isSuccess = false;

        try
        {

            //await _context.Database.ExecuteSqlRawAsync(
            //     "CALL ADDEditSelfOnlineEmploymentIncomeDetails({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})",
            //     selfOnlineEmploymentIncomeDetails.UserId,
            //     selfOnlineEmploymentIncomeDetails.UserId,
            //     selfOnlineEmploymentIncomeDetails.Year,
            //     "EmploymentIncome",
            //     selfOnlineEmploymentIncomeDetails.CategoryName,
            //     "Edit",
            //     selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentDetailsId,
            //     selfOnlineEmploymentIncomeDetails.Residency,
            //     selfOnlineEmploymentIncomeDetails.SeniorCitizen,
            //     selfOnlineEmploymentIncomeDetails.TypeOfName,
            //     selfOnlineEmploymentIncomeDetails.EmployerORCompanyName,
            //     selfOnlineEmploymentIncomeDetails.TINOfEmployer,
            //     selfOnlineEmploymentIncomeDetails.Remuneration,
            //     selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment,
            //     selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment,
            //     selfOnlineEmploymentIncomeDetails.TerminalBenefits,
            //     selfOnlineEmploymentIncomeDetails.Amount
            // );
            /*  var _employmentIncomuser = await _context.SelfOnlineEmploymentIncomeDetails
                               .Where(p => p.SelfOnlineEmploymentDetailsId == selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentDetailsId && p.UserId == selfOnlineEmploymentIncomeDetails.UserId && p.Year == selfOnlineEmploymentIncomeDetails.Year)
                               .FirstOrDefaultAsync();

              if (_employmentIncomuser != null)
              {
                  if (selfOnlineEmploymentIncomeDetails.CategoryName == "EmploymentDetails")
                  {
                      _employmentIncomuser.Residency = selfOnlineEmploymentIncomeDetails.Residency;
                      _employmentIncomuser.SeniorCitizen = selfOnlineEmploymentIncomeDetails.SeniorCitizen;
                      _employmentIncomuser.TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName;
                      _employmentIncomuser.EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName;
                      _employmentIncomuser.TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer;
                      _employmentIncomuser.Remuneration = selfOnlineEmploymentIncomeDetails.Remuneration;
                      _employmentIncomuser.APITPrimaryEmployment = selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment;
                      _employmentIncomuser.APITSecondaryEmployment = selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment;
                      _employmentIncomuser.UpdatedBy = selfOnlineEmploymentIncomeDetails.UserId;
                      _employmentIncomuser.UpdatedOn = DateTime.Now;

                  }
                  else if (selfOnlineEmploymentIncomeDetails.CategoryName == "TerminalBenefits")
                  {
                      _employmentIncomuser.TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName;
                      _employmentIncomuser.EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName;
                      _employmentIncomuser.TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer;
                      _employmentIncomuser.TerminalBenefits = selfOnlineEmploymentIncomeDetails.TerminalBenefits;
                      _employmentIncomuser.UpdatedBy = selfOnlineEmploymentIncomeDetails.UserId;
                      _employmentIncomuser.UpdatedOn = DateTime.Now;
                  }
                  else if (selfOnlineEmploymentIncomeDetails.CategoryName == "ExemptAmounts")
                  {
                      _employmentIncomuser.TypeOfName = selfOnlineEmploymentIncomeDetails.TypeOfName;
                      _employmentIncomuser.EmployerORCompanyName = selfOnlineEmploymentIncomeDetails.EmployerORCompanyName;
                      _employmentIncomuser.TINOfEmployer = selfOnlineEmploymentIncomeDetails.TINOfEmployer;
                      _employmentIncomuser.Amount = selfOnlineEmploymentIncomeDetails.Amount;
                      _employmentIncomuser.UpdatedBy = selfOnlineEmploymentIncomeDetails.UserId;
                      _employmentIncomuser.UpdatedOn = DateTime.Now;
                  }

                  await _context.SaveChangesAsync();*/
            //}
            //else
            //{

            //    return false;
            //}

            await _context.Database.ExecuteSqlRawAsync(
          @"CALL ADDEditSelfOnlineEmploymentIncomeDetails(
                @loguser,
                @UserId,
                @Year,
                @MainCategoryName,
                @CategoryName,
                @TransactionType,
                @SelfOnlineEmploymentDetailsId,
                @Residency,
                @SeniorCitizen,
                @TypeOfName,
                @EmployerORCompanyName,
                @TINOfEmployer,
                @Remuneration,
                @APITPrimaryEmployment,
                @APITSecondaryEmployment,
                @TerminalBenefits,
                @Amount
            )",
          new MySqlParameter("@loguser", selfOnlineEmploymentIncomeDetails.UserId ?? (object)DBNull.Value),
          new MySqlParameter("@UserId", selfOnlineEmploymentIncomeDetails.UserId ?? (object)DBNull.Value),
          new MySqlParameter("@Year", selfOnlineEmploymentIncomeDetails.Year),
          new MySqlParameter("@MainCategoryName", "EmploymentIncome"),
          new MySqlParameter("@CategoryName", selfOnlineEmploymentIncomeDetails.CategoryName ?? (object)DBNull.Value),
          new MySqlParameter("@TransactionType", "Edit" ?? (object)DBNull.Value),
          new MySqlParameter("@SelfOnlineEmploymentDetailsId", selfOnlineEmploymentIncomeDetails.SelfOnlineEmploymentDetailsId),
          new MySqlParameter("@Residency", selfOnlineEmploymentIncomeDetails.Residency),
          new MySqlParameter("@SeniorCitizen", selfOnlineEmploymentIncomeDetails.SeniorCitizen),
          new MySqlParameter("@TypeOfName", selfOnlineEmploymentIncomeDetails.TypeOfName ?? (object)DBNull.Value),
          new MySqlParameter("@EmployerORCompanyName", selfOnlineEmploymentIncomeDetails.EmployerORCompanyName ?? (object)DBNull.Value),
          new MySqlParameter("@TINOfEmployer", selfOnlineEmploymentIncomeDetails.TINOfEmployer ?? (object)DBNull.Value),
          new MySqlParameter("@Remuneration", selfOnlineEmploymentIncomeDetails.Remuneration ?? (object)DBNull.Value),
          new MySqlParameter("@APITPrimaryEmployment", selfOnlineEmploymentIncomeDetails.APITPrimaryEmployment ?? (object)DBNull.Value),
          new MySqlParameter("@APITSecondaryEmployment", selfOnlineEmploymentIncomeDetails.APITSecondaryEmployment ?? (object)DBNull.Value),
          new MySqlParameter("@TerminalBenefits", selfOnlineEmploymentIncomeDetails.TerminalBenefits ?? (object)DBNull.Value),
          new MySqlParameter("@Amount", selfOnlineEmploymentIncomeDetails.Amount ?? (object)DBNull.Value)
      );


            isSuccess = true;

        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

    public async Task<bool> DeleteEmploymentIncomeDetail(string userId, int year, int employmentDetailsId, string employmentDetailsName)
    {
        bool isSuccess = false;


        try
        {

            await _context.Database.ExecuteSqlRawAsync(
    @"CALL DeleteSelfOnlineEmploymentIncomeDetails  (
                @loguser,
                @UserId,
                @Year,
                @MainCategoryName,
                @CategoryName,
                @SelfOnlineEmploymentDetailsId
            )",
    new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
    new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
    new MySqlParameter("@Year", year),
    new MySqlParameter("@MainCategoryName", "EmploymentIncome"),
    new MySqlParameter("@CategoryName", employmentDetailsName),
    new MySqlParameter("@SelfOnlineEmploymentDetailsId", employmentDetailsId)
);
            //var _employmentIncomDetails = await _context.SelfOnlineEmploymentIncomeDetails
            //                  .Where(p => p.UserId == userId && p.Year == year && p.SelfOnlineEmploymentDetailsId == employmentDetailsId)
            //                  .FirstOrDefaultAsync();
            //if (_employmentIncomDetails != null)
            //{

            //    _context.SelfOnlineEmploymentIncomeDetails.Remove(_employmentIncomDetails);





            //    if (employmentDetailsName == "TerminalBenefits")
            //    {
            //        //     await _context.Database.ExecuteSqlRawAsync(
            //        //              "CALL UpdateSelfFilingTotalCalculation({0}, {1}, {2},{3},{4},{5},{6})",
            //        //              selfOnlineEmploymentIncomeDetails.UserId,
            //        //              selfOnlineEmploymentIncomeDetails.UserId,
            //        //              selfOnlineEmploymentIncomeDetails.Year,
            //        //              "EmploymentIncome",
            //        //              "EmploymentDetails",
            //        //              "add",
            //        //              addToTotal

            //        //          );
            //    }

            //}

            //await _context.SaveChangesAsync();

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update MaritalStatus");
        }
        return isSuccess;
    }
    public async Task<bool> SaveSelfOnlineInvestmentIncomeDetails(SelfOnlineInvestmentIncomeDto selfOnlineInvestment)
    {
        bool isSuccess = false;

        try
        {


            await _context.Database.ExecuteSqlRawAsync(
                  @"CALL ADDEditSelfOnlineInvestmentIncomeDetails(
                    @loguser,
                    @UserId,
                    @Year,
                    @MainCategoryName,
                    @Category,
                    @TransactionType,
                    @SelfOnlineInvestmentId,
                    @InvestmentIncomeType,
                    @Remuneration,
                    @GainsProfits,
                    @TotalInvestmentIncome,
                    @BankName,
                    @BankBranch,
                    @AccountNo,
                    @AmountInvested,
                    @Interest,
                    @OpeningBalance,
                    @Balance,
                    @CompanyInstitution,
                    @SharesStocks,
                    @AcquisitionDate,
                    @CostAcquisition,
                    @NetDividendIncome,
                    @PropertyDeedNo,
                    @RentAcquisitionDate,
                    @CostGiftInherited,
                    @MarketValue
                )",
                  new MySqlParameter("@loguser", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                  new MySqlParameter("@UserId", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                  new MySqlParameter("@Year", selfOnlineInvestment.Year),
                   new MySqlParameter("@MainCategoryName", "InvestmentIncome"),
                  new MySqlParameter("@Category", selfOnlineInvestment.Category ?? (object)DBNull.Value),
                  new MySqlParameter("@TransactionType", "Add"),
                   new MySqlParameter("@SelfOnlineInvestmentId", selfOnlineInvestment.SelfOnlineInvestmentId == 0 ? 0 : selfOnlineInvestment.SelfOnlineInvestmentId),
                  new MySqlParameter("@InvestmentIncomeType", selfOnlineInvestment.InvestmentIncomeType ?? (object)DBNull.Value),
                  new MySqlParameter("@Remuneration", selfOnlineInvestment.Remuneration ?? (object)DBNull.Value),
                  new MySqlParameter("@GainsProfits", selfOnlineInvestment.GainsProfits ?? (object)DBNull.Value),
                  new MySqlParameter("@TotalInvestmentIncome", selfOnlineInvestment.TotalInvestmentIncome ?? (object)DBNull.Value),
                   new MySqlParameter("@BankName", selfOnlineInvestment.BankName ?? (object)DBNull.Value),
                  new MySqlParameter("@BankBranch", selfOnlineInvestment.BankBranch ?? (object)DBNull.Value),
                  new MySqlParameter("@AccountNo", selfOnlineInvestment.AccountNo ?? (object)DBNull.Value),
                  new MySqlParameter("@AmountInvested", selfOnlineInvestment.AmountInvested ?? (object)DBNull.Value),
                  new MySqlParameter("@Interest", selfOnlineInvestment.Interest ?? (object)DBNull.Value),
                  new MySqlParameter("@OpeningBalance", selfOnlineInvestment.OpeningBalance ?? (object)DBNull.Value),
                  new MySqlParameter("@Balance", selfOnlineInvestment.Balance ?? (object)DBNull.Value),
                  new MySqlParameter("@CompanyInstitution", selfOnlineInvestment.CompanyInstitution ?? (object)DBNull.Value),
                  new MySqlParameter("@SharesStocks", selfOnlineInvestment.SharesStocks ?? (object)DBNull.Value),
                  new MySqlParameter("@AcquisitionDate", selfOnlineInvestment.AcquisitionDate ?? (object)DBNull.Value),
                  new MySqlParameter("@CostAcquisition", selfOnlineInvestment.CostAcquisition ?? (object)DBNull.Value),
                  new MySqlParameter("@NetDividendIncome", selfOnlineInvestment.NetDividendIncome ?? (object)DBNull.Value),
                  new MySqlParameter("@PropertyDeedNo", selfOnlineInvestment.PropertyDeedNo ?? (object)DBNull.Value),
                  new MySqlParameter("@RentAcquisitionDate", selfOnlineInvestment.RentAcquisitionDate ?? (object)DBNull.Value),
                  new MySqlParameter("@CostGiftInherited", selfOnlineInvestment.CostGiftInherited ?? (object)DBNull.Value),
                  new MySqlParameter("@MarketValue", selfOnlineInvestment.MarketValue ?? (object)DBNull.Value)
              );

            isSuccess = true;

        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }

}


