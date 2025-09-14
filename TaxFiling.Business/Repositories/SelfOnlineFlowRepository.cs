using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Diagnostics.Metrics;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public async Task<List<MaritalStatusDetailsDto>> GetMaritalStatus(string userId, int year, CancellationToken ctx)
    {

        try
        {
            var maritalStatus = await (
            from m in _context.MaritalStatuses
            join s in _context.SelfOnlineFlowPersonalInformation
                on new { MaritalStatusId = m.Id, UserId = userId, Year = year }
                equals new { MaritalStatusId = s.MaritalStatusId ?? 0, s.UserId, s.Year }
                into ps
            from s in ps.DefaultIfEmpty()
            select new MaritalStatusDetailsDto
            {
                UserId = s != null ? s.UserId : userId,
                Year = s != null ? s.Year : year,
                Id = m.Id,
                Name = m.Name,
                ImageUrl = m.ImageUrl,
                NumberOfDependents = s.NumberOfDependents,
                SpouseFullName = s != null ? s.SpouseName : null,
                SpouseTINNo = s != null ? s.SpouseTINNo : null,
                SpouseNIC = s != null ? s.SpouseNIC : null,

            }
            ).ToListAsync(ctx);

            return maritalStatus;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
            return new List<MaritalStatusDetailsDto>();
        }

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
                                            Relationship = b.Relationship,
                                            Title = b.Title,
                                            PassportNo = b.PassportNo,
                                            Nationality = b.Nationality,
                                            Occupation = b.Occupation,
                                            EmployerName = b.EmployerName,
                                            District = b.District,
                                            PostalCode = b.PostalCode,
                                            Country = b.Country,
                                            EmailPrimary = b.EmailPrimary,
                                            EmailSecondary = b.EmailSecondary,
                                            MobilePhone = b.MobilePhone,
                                            HomePhone = b.HomePhone,
                                            WhatsApp = b.WhatsApp,
                                            PreferredCommunicationMethod = b.PreferredCommunicationMethod,
                                            NumberOfDependents = b.NumberOfDependents
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

    public async Task<bool> UpdateMaritalStatus(MaritalStatusDetailsDto maritalStatusDetails)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == maritalStatusDetails.UserId && p.Year == maritalStatusDetails.Year)
                                .FirstOrDefaultAsync();

            if (maritalStatusDetails.Id != 2)
            {
                maritalStatusDetails.SpouseFullName = null;
                maritalStatusDetails.SpouseTINNo = null;
                maritalStatusDetails.SpouseNIC = null;
            }

            _user.MaritalStatusId = maritalStatusDetails.Id;
            _user.SpouseName = maritalStatusDetails.SpouseFullName;
            _user.SpouseTINNo = maritalStatusDetails.SpouseTINNo;
            _user.SpouseNIC = maritalStatusDetails.SpouseNIC;
            _user.NumberOfDependents = maritalStatusDetails.NumberOfDependents;

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
                _selfOnlineuser.Title = identifications.Title;
                _selfOnlineuser.PassportNo = identifications.PassportNo;
                _selfOnlineuser.Nationality = identifications.Nationality;
                _selfOnlineuser.Occupation = identifications.Occupation;
                _selfOnlineuser.EmployerName = identifications.EmployerName;

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

    public async Task<bool> UpdatelContactInformation(ContactInfromationDto contactInfromation)
    {
        bool isSuccess = false;

        try
        {
            var _selfOnlineuser = await _context.SelfOnlineFlowPersonalInformation
                               .Where(p => p.UserId == contactInfromation.UserId && p.Year == contactInfromation.Year)
                               .FirstOrDefaultAsync();

            if (_selfOnlineuser != null)
            {
                _selfOnlineuser.CareOf = contactInfromation.CareOf;
                _selfOnlineuser.Apt = contactInfromation.Apt;
                _selfOnlineuser.StreetNumber = contactInfromation.StreetNumber;
                _selfOnlineuser.Street = contactInfromation.Street;
                _selfOnlineuser.City = contactInfromation.City;
                _selfOnlineuser.District = contactInfromation.District;
                _selfOnlineuser.PostalCode = contactInfromation.PostalCode;
                _selfOnlineuser.Country = contactInfromation.Country;
                _selfOnlineuser.EmailPrimary = contactInfromation.EmailPrimary;
                _selfOnlineuser.EmailSecondary = contactInfromation.EmailSecondary;
                _selfOnlineuser.MobilePhone = contactInfromation.MobilePhone;
                _selfOnlineuser.HomePhone = contactInfromation.HomePhone;
                _selfOnlineuser.WhatsApp = contactInfromation.WhatsApp;
                _selfOnlineuser.PreferredCommunicationMethod = contactInfromation.PreferredCommunicationMethod;

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update Contact Information");
        }
        return isSuccess;
    }

    public async Task<SelfFilingTotalCalculationDto?> GetSelfFilingTotalCalculation(string userId, int year, CancellationToken ctx)
    {
        var totalCalculation = await _context.SelfFilingTotalCalculation
                            .AsNoTracking()
                            .Where(p => p.UserId == userId && p.Year == year)
                            .Select(p => new SelfFilingTotalCalculationDto
                            {
                                UserId = p.UserId,
                                EmploymentIncomeTotal = p.EmploymentIncomeTotal,
                                EmpIncome_EmpDetails = p.EmpIncome_EmpDetails,
                                EmpIncome_TermBenefits = p.EmpIncome_TermBenefits,
                                EmpIncome_ExeAmounts = p.EmpIncome_ExeAmounts,
                                InvestmentIncomeTotal = p.InvestmentIncomeTotal,
                                InvIncome_Savings = p.InvIncome_Savings,
                                InvIncome_FixedDeposit = p.InvIncome_FixedDeposit,
                                InvIncome_Dividend = p.InvIncome_Dividend,
                                InvIncome_Rent = p.InvIncome_Rent,
                                InvIncome_Partner = p.InvIncome_Partner,
                                InvIncome_Beneficiary = p.InvIncome_Beneficiary,
                                InvIncome_ExemptAmounts = p.InvIncome_ExemptAmounts,
                                InvIncome_Other = p.InvIncome_Other,
                                ReliefSolarPanel = p.ReliefSolarPanel,
                                QualifyingPayments =p.QualifyingPayments
                            })
                            .FirstOrDefaultAsync(ctx);



        return totalCalculation;
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
                                            Total = b.Total,
                                            BenefitExcludedForTax = b.BenefitExcludedForTax

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
                @Amount,
                @BenefitExcludedForTax
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
            new MySqlParameter("@Amount", selfOnlineEmploymentIncomeDetails.Amount ?? (object)DBNull.Value),
            new MySqlParameter("@BenefitExcludedForTax", selfOnlineEmploymentIncomeDetails.BenefitExcludedForTax ?? (object)DBNull.Value)
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

             employmentIncome = await (
                   from e in _context.SelfOnlineEmploymentIncomeDetails
                   join t in _context.Users
                       on e.UserId equals t.UserId.ToString()
                   where e.UserId == userId && e.Year == year
                   select new SelfOnlineEmploymentIncomeDetailsDto
                    {
                        SelfOnlineEmploymentDetailsId = e.SelfOnlineEmploymentDetailsId,
                        Residency = e.Residency,
                        SeniorCitizen = e.SeniorCitizen,
                        CategoryName = e.CategoryName,
                        TypeOfName = e.TypeOfName,
                        EmployerORCompanyName = e.EmployerORCompanyName,
                        TINOfEmployer = e.TINOfEmployer,
                        Remuneration = e.Remuneration,
                        APITPrimaryEmployment = e.APITPrimaryEmployment,
                        APITSecondaryEmployment = e.APITSecondaryEmployment,
                        TerminalBenefits = e.TerminalBenefits,
                        Amount = e.Amount,
                        Total = t.TaxTotal,
                        BenefitExcludedForTax = e.BenefitExcludedForTax
                    }
                ).ToListAsync(ctx);


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
                @Amount,
                @BenefitExcludedForTax
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
          new MySqlParameter("@Amount", selfOnlineEmploymentIncomeDetails.Amount ?? (object)DBNull.Value),
          new MySqlParameter("@BenefitExcludedForTax", selfOnlineEmploymentIncomeDetails.BenefitExcludedForTax ?? (object)DBNull.Value)
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
                     @MarketValue,
                     @PBTotalInvestmentIncome,
                     @ActivityCode,
                     @PartnershipName,
                     @TrustTIN,
                     @PBGainsProfits,
                     @TotalInvestmentIncomePartnership,
                     @TotalInvestmentIncomeTrust,
                     @IsExemptAmountA,
                     @IsExcludedAmountB,
                     @ExemptExcludedIncome
                 )",
                 new MySqlParameter("@loguser", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                 new MySqlParameter("@UserId", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                 new MySqlParameter("@Year", selfOnlineInvestment.Year),
                 new MySqlParameter("@Category", selfOnlineInvestment.Category ?? (object)DBNull.Value),
                 new MySqlParameter("@TransactionType", selfOnlineInvestment.TransactionType ?? (object)DBNull.Value),
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
                 new MySqlParameter("@MarketValue", selfOnlineInvestment.MarketValue ?? (object)DBNull.Value),
                 new MySqlParameter("@PBTotalInvestmentIncome", selfOnlineInvestment.PBTotalInvestmentIncome ?? (object)DBNull.Value),
                 new MySqlParameter("@ActivityCode", selfOnlineInvestment.ActivityCode ?? (object)DBNull.Value),
                 new MySqlParameter("@PartnershipName", selfOnlineInvestment.PartnershipName ?? (object)DBNull.Value),
                 new MySqlParameter("@TrustTIN", selfOnlineInvestment.TrustTIN ?? (object)DBNull.Value),
                 new MySqlParameter("@PBGainsProfits", selfOnlineInvestment.PBGainsProfits ?? (object)DBNull.Value),
                 new MySqlParameter("@TotalInvestmentIncomePartnership", selfOnlineInvestment.TotalInvestmentIncomePartnership ?? (object)DBNull.Value),
                 new MySqlParameter("@TotalInvestmentIncomeTrust", selfOnlineInvestment.TotalInvestmentIncomeTrust ?? (object)DBNull.Value),
                 new MySqlParameter("@IsExemptAmountA", selfOnlineInvestment.IsExemptAmountA ?? (object)DBNull.Value),
                 new MySqlParameter("@IsExcludedAmountB", selfOnlineInvestment.IsExcludedAmountB ?? (object)DBNull.Value),
                 new MySqlParameter("@ExemptExcludedIncome", selfOnlineInvestment.ExemptExcludedIncome ?? (object)DBNull.Value)
             );

            isSuccess = true;

        }

        catch (Exception e)
        {
            _logger.LogError(e, "Error update LastYear");
        }
        return isSuccess;
    }
    public async Task<List<SelfOnlineInvestmentIncomeDto>> GetSelfOnlineInvestmentIncomeList(string userId, int year, CancellationToken ctx)
    {
        List<SelfOnlineInvestmentIncomeDto> investmentIncome = [];
        try
        {

            investmentIncome = await _context.SelfOnlineInvestmentIncome
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfOnlineInvestmentIncomeDto
            {
                SelfOnlineInvestmentId = t.SelfOnlineInvestmentId,
                UserId = t.UserId,
                Year = t.Year,
                Category = t.Category,
                InvestmentIncomeType = t.InvestmentIncomeType,
                Remuneration = t.Remuneration,
                GainsProfits = t.GainsProfits,
                TotalInvestmentIncome = t.TotalInvestmentIncome,
                BankName = t.BankName,
                BankBranch = t.BankBranch,
                AccountNo = t.AccountNo,
                AmountInvested = t.AmountInvested,
                Interest = t.Interest,
                OpeningBalance = t.OpeningBalance,
                Balance = t.Balance,
                CompanyInstitution = t.CompanyInstitution,
                SharesStocks = t.SharesStocks,
                AcquisitionDate = t.AcquisitionDate,
                CostAcquisition = t.CostAcquisition,
                NetDividendIncome = t.NetDividendIncome,
                PropertyDeedNo = t.PropertyDeedNo,
                RentAcquisitionDate = t.RentAcquisitionDate,
                CostGiftInherited = t.CostGiftInherited,
                MarketValue = t.MarketValue


            }).ToListAsync(ctx);


        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return investmentIncome;
    }

    public async Task<bool> DeleteInvestmentIncomeDetail(string userId, int year, int investmentIncomeId, string categoryName)
    {
        bool isSuccess = false;


        try
        {

            await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteSelfOnlineInvestmentIncomeDetails  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @MainCategoryName,
                                        @CategoryName,
                                        @SelfOnlineInvestmentDetailsId
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@MainCategoryName", "InvestmentIncome"),
                            new MySqlParameter("@CategoryName", categoryName),
                            new MySqlParameter("@SelfOnlineInvestmentDetailsId", investmentIncomeId)
                );

            isSuccess = true;


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Delete Investment Income Detail");
        }
        return isSuccess;
    }

    //----------------------------new 
    public async Task<bool> SaveSelfOnlineInvestmentDetails(SelfOnlineInvestmentIncomeDetailDto selfOnlineInvestment)
    {
        bool isSuccess = false;

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL ADDEditSelfOnlineInvestmentIncomeDetailsV1(
                @loguser,
                @UserId,
                @year,
                @Category,
                @transactionType,
                @SelfOnlineInvestmentDetailsId,
                @ActivityCode,
                @TypeOfInvestment,
                @AmountInvested,
                @IncomeAmount,
                @WHTDeducted,
                @ForeignTaxCredit,
                @BankName,
                @BankBranch,
                @AccountNo,
                @WHTCertificateNo,
                @OpeningBalance,
                @ClosingBalance,
                @CompanyInstitution,
                @SharesStocks,
                @AcquisitionDate,
                @CostAcquisition,
                @NetDividendIncome,
                @PropertySituation,
                @PropertyAddress,
                @DeedNo,
                @RatesLocalAuthority,
                @GiftOrInheritedCost,
                @MarketValue
            )",
                new MySqlParameter("@loguser", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                new MySqlParameter("@UserId", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                new MySqlParameter("@year", selfOnlineInvestment.Year),
                new MySqlParameter("@Category", selfOnlineInvestment.Category ?? (object)DBNull.Value),
                new MySqlParameter("@transactionType", selfOnlineInvestment.TransactionType ?? (object)DBNull.Value),
                new MySqlParameter("@SelfOnlineInvestmentDetailsId", selfOnlineInvestment.InvestmentIncomeDetailId == 0 ? 0 : selfOnlineInvestment.InvestmentIncomeDetailId),
                new MySqlParameter("@ActivityCode", selfOnlineInvestment.ActivityCode ?? (object)DBNull.Value),
                new MySqlParameter("@TypeOfInvestment", selfOnlineInvestment.TypeOfInvestment ?? (object)DBNull.Value),
                new MySqlParameter("@AmountInvested", selfOnlineInvestment.AmountInvested ?? (object)DBNull.Value),
                new MySqlParameter("@IncomeAmount", selfOnlineInvestment.IncomeAmount ?? (object)DBNull.Value),
                new MySqlParameter("@WHTDeducted", selfOnlineInvestment.WHTDeducted ?? (object)DBNull.Value),
                new MySqlParameter("@ForeignTaxCredit", selfOnlineInvestment.ForeignTaxCredit ?? (object)DBNull.Value),
                new MySqlParameter("@BankName", selfOnlineInvestment.BankName ?? (object)DBNull.Value),
                new MySqlParameter("@BankBranch", selfOnlineInvestment.BankBranch ?? (object)DBNull.Value),
                new MySqlParameter("@AccountNo", selfOnlineInvestment.AccountNo ?? (object)DBNull.Value),
                new MySqlParameter("@WHTCertificateNo", selfOnlineInvestment.WHTCertificateNo ?? (object)DBNull.Value),
                new MySqlParameter("@OpeningBalance", selfOnlineInvestment.OpeningBalance ?? (object)DBNull.Value),
                new MySqlParameter("@ClosingBalance", selfOnlineInvestment.ClosingBalance ?? (object)DBNull.Value),
                new MySqlParameter("@CompanyInstitution", selfOnlineInvestment.CompanyInstitution ?? (object)DBNull.Value),
                new MySqlParameter("@SharesStocks", selfOnlineInvestment.SharesStocks ?? (object)DBNull.Value),
                new MySqlParameter("@AcquisitionDate", selfOnlineInvestment.AcquisitionDate ?? (object)DBNull.Value),
                new MySqlParameter("@CostAcquisition", selfOnlineInvestment.CostAcquisition ?? (object)DBNull.Value),
                new MySqlParameter("@NetDividendIncome", selfOnlineInvestment.NetDividendIncome ?? (object)DBNull.Value),
                new MySqlParameter("@PropertySituation", selfOnlineInvestment.PropertySituation ?? (object)DBNull.Value),
                new MySqlParameter("@PropertyAddress", selfOnlineInvestment.PropertyAddress ?? (object)DBNull.Value),
                new MySqlParameter("@DeedNo", selfOnlineInvestment.DeedNo ?? (object)DBNull.Value),
                new MySqlParameter("@RatesLocalAuthority", selfOnlineInvestment.RatesLocalAuthority ?? (object)DBNull.Value),
                new MySqlParameter("@GiftOrInheritedCost", selfOnlineInvestment.GiftOrInheritedCost ?? (object)DBNull.Value),
                new MySqlParameter("@MarketValue", selfOnlineInvestment.MarketValue ?? (object)DBNull.Value)
            );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveSelfOnlineInvestmentDetails");
        }

        return isSuccess;
    }
    public async Task<List<SelfOnlineInvestmentIncomeDetailDto>> GetSelfOnlineInvestmentIncomeDetailsList(string userId, int year, CancellationToken ctx)
    {
        List<SelfOnlineInvestmentIncomeDetailDto> investmentIncome = [];
        try
        {

            investmentIncome = await _context.SelfOnlineInvestmentIncomeDetail
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfOnlineInvestmentIncomeDetailDto
            {
                InvestmentIncomeDetailId = t.InvestmentIncomeDetailId,
                UserId = t.UserId,
                Year = t.Year,
                Category = t.Category,

                ActivityCode = t.ActivityCode,
                TypeOfInvestment = t.TypeOfInvestment,

                AmountInvested = t.AmountInvested,
                IncomeAmount = t.IncomeAmount,
                WHTDeducted = t.WHTDeducted,
                ForeignTaxCredit = t.ForeignTaxCredit,

                BankName = t.BankName,
                BankBranch = t.BankBranch,
                AccountNo = t.AccountNo,
                WHTCertificateNo = t.WHTCertificateNo,
                OpeningBalance = t.OpeningBalance,
                ClosingBalance = t.ClosingBalance,

                CompanyInstitution = t.CompanyInstitution,
                SharesStocks = t.SharesStocks,
                AcquisitionDate = t.AcquisitionDate,
                CostAcquisition = t.CostAcquisition,
                NetDividendIncome = t.NetDividendIncome,

                PropertySituation = t.PropertySituation,
                PropertyAddress = t.PropertyAddress,
                DeedNo = t.DeedNo,
                RatesLocalAuthority = t.RatesLocalAuthority,
                GiftOrInheritedCost = t.GiftOrInheritedCost,
                MarketValue = t.MarketValue
            })
            .ToListAsync(ctx);




        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return investmentIncome;
    }

    public async Task<bool> DeleteSelfOnlineInvestmentDetails(string userId, int year, int investmentIncomeId, string categoryName)
    {
        bool isSuccess = false;


        try
        {

            await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteselfOnlineinvestmentincomeDetailsV1  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @MainCategoryName,
                                        @CategoryName,
                                        @SelfOnlineInvestmentDetailsId
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@MainCategoryName", "InvestmentIncome"),
                            new MySqlParameter("@CategoryName", categoryName),
                            new MySqlParameter("@SelfOnlineInvestmentDetailsId", investmentIncomeId)
                );


        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Delete Investment Income Detail");
        }
        return isSuccess;
    }

    public async Task<bool> SaveSelfOnlineInvestmentPartnerBeneficiaryExempt(SelfOnlineInvestmentPartnerBeneficiaryExemptDto selfOnlineInvestment)
    {
        bool isSuccess = false;

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                   @"CALL ADDEditSelfOnlineInvestmentPartnerBeneficiaryExempt(
                        @loguser,
                        @UserId,
                        @year,
                        @Category,
                        @transactionType,
                        @InvestmentIncomePBEId,
                        @TotalInvestmentIncome,
                        @ActivityCode,
                        @PartnershipName,
                        @TrustName,
                        @TINNO,
                        @GainsProfits,
                        @TotalInvestmentIncomePartnership,
                        @TotalInvestmentIncomeTrust,
                        @IsExemptAmountA,
                        @IsExcludedAmountB,
                        @ExemptExcludedIncome
                    )",
                       new MySqlParameter("@loguser", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                       new MySqlParameter("@UserId", selfOnlineInvestment.UserId ?? (object)DBNull.Value),
                       new MySqlParameter("@year", selfOnlineInvestment.Year),
                       new MySqlParameter("@Category", selfOnlineInvestment.Category ?? (object)DBNull.Value),
                       new MySqlParameter("@transactionType", selfOnlineInvestment.TransactionType ?? (object)DBNull.Value),
                       new MySqlParameter("@InvestmentIncomePBEId", selfOnlineInvestment.InvestmentIncomePBEId),
                       new MySqlParameter("@TotalInvestmentIncome", selfOnlineInvestment.TotalInvestmentIncome ?? (object)DBNull.Value),
                       new MySqlParameter("@ActivityCode", selfOnlineInvestment.ActivityCode ?? (object)DBNull.Value),
                       new MySqlParameter("@PartnershipName", selfOnlineInvestment.PartnershipName ?? (object)DBNull.Value),
                       new MySqlParameter("@TrustName", selfOnlineInvestment.TrustName ?? (object)DBNull.Value),
                       new MySqlParameter("@TINNO", selfOnlineInvestment.TINNO ?? (object)DBNull.Value),
                       new MySqlParameter("@GainsProfits", selfOnlineInvestment.GainsProfits ?? (object)DBNull.Value),
                       new MySqlParameter("@TotalInvestmentIncomePartnership", selfOnlineInvestment.TotalInvestmentIncomePartnership ?? (object)DBNull.Value),
                       new MySqlParameter("@TotalInvestmentIncomeTrust", selfOnlineInvestment.TotalInvestmentIncomeTrust ?? (object)DBNull.Value),
                       new MySqlParameter("@IsExemptAmountA", selfOnlineInvestment.IsExemptAmountA ?? (object)DBNull.Value),
                       new MySqlParameter("@IsExcludedAmountB", selfOnlineInvestment.IsExcludedAmountB ?? (object)DBNull.Value),
                       new MySqlParameter("@ExemptExcludedIncome", selfOnlineInvestment.ExemptExcludedIncome ?? (object)DBNull.Value)
                   );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveSelfOnlineInvestmentPartnerBeneficiaryExempt");
        }

        return isSuccess;
    }
    public async Task<List<SelfOnlineInvestmentPartnerBeneficiaryExemptDto>> GetSelfOnlineInvestmentPartnerBeneficiaryExempt(string userId, int year, CancellationToken ctx)
    {
        List<SelfOnlineInvestmentPartnerBeneficiaryExemptDto> investmentIncome = [];
        try
        {

            investmentIncome = await _context.SelfOnlineInvestmentPartnerBeneficiaryExempt
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfOnlineInvestmentPartnerBeneficiaryExemptDto
            {
                InvestmentIncomePBEId = t.InvestmentIncomePBEId,
                UserId = t.UserId,
                Year = t.Year,
                Category = t.Category,
                TotalInvestmentIncome = t.TotalInvestmentIncome,
                ActivityCode = t.ActivityCode,
                PartnershipName = t.PartnershipName,
                TrustName = t.TrustName,
                TINNO = t.TINNO,
                GainsProfits = t.GainsProfits,
                TotalInvestmentIncomePartnership = t.TotalInvestmentIncomePartnership,
                TotalInvestmentIncomeTrust = t.TotalInvestmentIncomeTrust,
                IsExemptAmountA = t.IsExemptAmountA,
                IsExcludedAmountB = t.IsExcludedAmountB,
                ExemptExcludedIncome = t.ExemptExcludedIncome

            })
            .ToListAsync(ctx);




        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return investmentIncome;
    }

    public async Task<bool> DeleteSelfOnlineInvestmentPartnerBeneficiaryExempt(string userId, int year, int investmentIncomeId, string categoryName)
    {
        bool isSuccess = false;


        try
        {

            await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteselfOnlineinvestmentpartnerbeneficiaryexempt  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @MainCategoryName,
                                        @CategoryName,
                                        @SelfOnlineInvestmentIncomePBEId
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@MainCategoryName", "InvestmentIncome"),
                            new MySqlParameter("@CategoryName", categoryName),
                            new MySqlParameter("@SelfOnlineInvestmentIncomePBEId", investmentIncomeId)
                );
            isSuccess = true;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Delete Investment Partner Beneficiary Exempt Income Detail");
        }
        return isSuccess;
    }

    public async Task<bool> UpdateSelfFilingTotalCalculation(SelfFilingTotalCalculationDto totalCalculation)
    {
        bool isSuccess = false;

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                  @"CALL ADDEditSelfOnlineAdditionalDetails  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @CategoryName,
                                        @ReliefSolarPanel,
                                        @QualifyingPayments
                                    )",
                          new MySqlParameter("@loguser", totalCalculation.UserId ?? (object)DBNull.Value),
                          new MySqlParameter("@UserId", totalCalculation.UserId ?? (object)DBNull.Value),
                          new MySqlParameter("@Year", totalCalculation.Year),
                          new MySqlParameter("@CategoryName", "Deductions"),
                          new MySqlParameter("@ReliefSolarPanel", totalCalculation.ReliefSolarPanel),
                          new MySqlParameter("@QualifyingPayments", totalCalculation.QualifyingPayments)
              );
            isSuccess = true;
            

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error update UpdateSelfFilingTotalCalculation");
        }
        return isSuccess;
    }

    //-------- Assets and Liabilities

    //-------- Assets

    public async Task<bool> SaveSelfonlineAssetsImmovableProperty(SelfonlineAssetsImmovablePropertyDto immovableProperties)
    {
        bool isSuccess = false;
        try
            {
            await _context.Database.ExecuteSqlRawAsync(
                  @"CALL ADDEditSelfOnlineImmovableProperties  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @Situation,
                                        @DateOfAcquisition,
                                        @Cost,
                                        @MarketValue,
                                        @transactionType,
                                        @SelfonlinePropertyID
                                    )",
                          new MySqlParameter("@loguser", immovableProperties.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@UserId", immovableProperties.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@Year", immovableProperties.Year),
                        new MySqlParameter("@Type", immovableProperties.Type ?? (object)DBNull.Value),
                        new MySqlParameter("@SerialNumber", immovableProperties.SerialNumber ?? (object)DBNull.Value),
                        new MySqlParameter("@Situation", immovableProperties.Situation ?? (object)DBNull.Value),
                        new MySqlParameter("@DateOfAcquisition", immovableProperties.DateOfAcquisition ?? (object)DBNull.Value),
                        new MySqlParameter("@Cost", immovableProperties.Cost ?? (object)DBNull.Value),
                        new MySqlParameter("@MarketValue", immovableProperties.MarketValue ?? (object)DBNull.Value),
                        new MySqlParameter("@transactionType", immovableProperties.TransactionType ?? (object)DBNull.Value),
                        new MySqlParameter("@SelfonlinePropertyID", immovableProperties.SelfonlinePropertyID)

              );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveSelfonlineAssetsMotorVehicle");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineAssetsImmovablePropertyDto>> GetSelfOnlineAssetsImmovableProperty(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineAssetsImmovablePropertyDto> immovablePropertyList = [];
        try
        {

            immovablePropertyList = await _context.SelfonlineAssetsImmovableProperty
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineAssetsImmovablePropertyDto
            {
                SelfonlinePropertyID = t.SelfonlinePropertyID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                Situation = t.Situation,
                DateOfAcquisition = t.DateOfAcquisition,
                Cost = t.Cost,
                MarketValue = t.MarketValue

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return immovablePropertyList;
    }

    public async Task<bool> SaveSelfonlineAssetsMotorVehicle(SelfonlineAssetsMotorVehicleDto motorVehicles)
    {
        bool isSuccess = false;
        try
        {


                    await _context.Database.ExecuteSqlRawAsync(
                                          @"CALL ADDEdit_SelfOnlineMotorVehicle (
                                            @loguser,
                                            @UserId,
                                            @Year,
                                            @transactionType,
                                            @SelfonlineMotorVehicleID,
                                            @Type,
                                            @SerialNumber,
                                            @Description,
                                            @RegistrationNo,
                                            @DateOfAcquisition,
                                            @CostMarketValue
                                        )",
                                          new MySqlParameter("@loguser", motorVehicles.UserId),  // <-- you probably meant logUser, not UserId?
                                          new MySqlParameter("@UserId", motorVehicles.UserId),
                                          new MySqlParameter("@Year", motorVehicles.Year),
                                          new MySqlParameter("@transactionType", motorVehicles.TransactionType ?? (object)DBNull.Value),
                                          new MySqlParameter("@SelfonlineMotorVehicleID", motorVehicles.SelfonlineMotorVehicleID),
                                          new MySqlParameter("@Type", motorVehicles.Type ?? (object)DBNull.Value),
                                          new MySqlParameter("@SerialNumber", motorVehicles.SerialNumber ?? (object)DBNull.Value),
                                          new MySqlParameter("@Description", motorVehicles.Description ?? (object)DBNull.Value),
                                          new MySqlParameter("@RegistrationNo", motorVehicles.RegistrationNo ?? (object)DBNull.Value),
                                          new MySqlParameter("@DateOfAcquisition", motorVehicles.DateOfAcquisition ?? (object)DBNull.Value),
                                          new MySqlParameter("@CostMarketValue", motorVehicles.CostMarketValue ?? (object)DBNull.Value)
          );


            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveSelfonlineAssetsMotorVehicle");
        }

        return isSuccess;
    }

    public async Task<List<SelfonlineAssetsMotorVehicleDto>> GetSelfOnlineAssetsMotorVehicle(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineAssetsMotorVehicleDto> mtorVehicleList = [];
        try
        {

            mtorVehicleList = await _context.SelfonlineAssetsMotorVehicle
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineAssetsMotorVehicleDto
            {
                SelfonlineMotorVehicleID = t.SelfonlineMotorVehicleID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                Description = t.Description,
                RegistrationNo = t.RegistrationNo,
                DateOfAcquisition = t.DateOfAcquisition,
                CostMarketValue = t.CostMarketValue

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return mtorVehicleList;
    }

    public async Task<bool> DeleteSelfOnlinAssetsDtails(string userId, int year, int deleteAssetsId, string categoryName)
    {
        bool isSuccess = false;
        try
        {
            if(categoryName == "ImmovableProperty")
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteSelfOnlineImmovableProperty  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @SelfonlinePropertyID
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@SelfonlinePropertyID", deleteAssetsId)
                );
            }
            if (categoryName == "MotorVehicle")
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteSelfOnlineMotorVehicle   (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @SelfonlineMotorVehicleID
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@SelfonlineMotorVehicleID", deleteAssetsId)
                );
            }
            if (categoryName == "SharesStocksSecurities")
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteSelfOnlineSharesStocksSecurities   (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @SelfonlineSharesStocksID
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@SelfonlineSharesStocksID", deleteAssetsId)
                );
            }
            if (categoryName == "CapitalCurrentAccount")
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL DeleteSelfOnlineCapitalCurrentAccount (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @SelfonlineBusinessAccountID
                                    )",
                            new MySqlParameter("@loguser", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@UserId", userId ?? (object)DBNull.Value),
                            new MySqlParameter("@Year", year),
                            new MySqlParameter("@SelfonlineBusinessAccountID", deleteAssetsId)
                );
            }

            isSuccess = true;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Delete  Detail");
        }
        return isSuccess;
    }

    public async Task<bool> SaveEditSelfonlineAssetsSharesStocksSecurities(SelfonlineAssetsSharesStocksSecuritiesDto sharesStockStocksSecurities)
    {
        bool isSuccess = false;
        try
        {
 
            await _context.Database.ExecuteSqlRawAsync(
                  @"CALL ADDEditSelfOnlineSharesStocks  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @CompanyName,
                                        @NoOfSharesStocks,
                                        @DateOfAcquisition,
                                        @CostOfAcquisition,
                                        @NetDividendIncome,
                                        @transactionType,
                                        @SelfonlineSharesStocksID
                                    )",
                         new MySqlParameter("@loguser", sharesStockStocksSecurities.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@UserId", sharesStockStocksSecurities.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@Year", sharesStockStocksSecurities.Year),
                        new MySqlParameter("@Type", sharesStockStocksSecurities.Type ?? (object)DBNull.Value),
                        new MySqlParameter("@SerialNumber", sharesStockStocksSecurities.SerialNumber ?? (object)DBNull.Value),
                        new MySqlParameter("@CompanyName", sharesStockStocksSecurities.CompanyName ?? (object)DBNull.Value),
                        new MySqlParameter("@NoOfSharesStocks", sharesStockStocksSecurities.NoOfSharesStocks ?? (object)DBNull.Value),
                        new MySqlParameter("@DateOfAcquisition", sharesStockStocksSecurities.DateOfAcquisition ?? (object)DBNull.Value),
                        new MySqlParameter("@CostOfAcquisition", sharesStockStocksSecurities.CostOfAcquisition ?? (object)DBNull.Value),
                        new MySqlParameter("@NetDividendIncome", sharesStockStocksSecurities.NetDividendIncome ?? (object)DBNull.Value),
                         new MySqlParameter("@transactionType", sharesStockStocksSecurities.TransactionType ?? (object)DBNull.Value),
                        new MySqlParameter("@SelfonlineSharesStocksID", sharesStockStocksSecurities.SelfonlineSharesStocksID)

              );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveSelfonlineAssetsMotorVehicle");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineAssetsSharesStocksSecuritiesDto>> GetSelfOnlineAssetsSharesStocksSecurities(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineAssetsSharesStocksSecuritiesDto> sharesStocksSecuritiesList = [];
        try
        {

            sharesStocksSecuritiesList = await _context.SelfonlineAssetsSharesStocksSecurities
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineAssetsSharesStocksSecuritiesDto
            {
                SelfonlineSharesStocksID = t.SelfonlineSharesStocksID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                CompanyName = t.CompanyName,
                NoOfSharesStocks = t.NoOfSharesStocks,
                DateOfAcquisition = t.DateOfAcquisition,
                CostOfAcquisition = t.CostOfAcquisition,
                NetDividendIncome = t.NetDividendIncome

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return sharesStocksSecuritiesList;
    }

    public async Task<bool> SaveEditSelfonlineAssetsCapitalCurrentAccount(SelfonlineAssetsCapitalCurrentAccountDto capitalCurrentAccount)
    {
        bool isSuccess = false;
        try
        {
  
                await _context.Database.ExecuteSqlRawAsync(
                  @"CALL ADDEditSelfOnlineCapitalCurrentAccount  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @BusinessName,
                                        @CurrentAccountBalance,
                                        @CapitalAccountBalance,
                                        @transactionType,
                                        @SelfonlineBusinessAccountID
                                    )",
                         new MySqlParameter("@loguser", capitalCurrentAccount.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@UserId", capitalCurrentAccount.UserId ?? (object)DBNull.Value),
                        new MySqlParameter("@Year", capitalCurrentAccount.Year),
                        new MySqlParameter("@Type", capitalCurrentAccount.Type ?? (object)DBNull.Value),
                        new MySqlParameter("@SerialNumber", capitalCurrentAccount.SerialNumber ?? (object)DBNull.Value),
                        new MySqlParameter("@BusinessName", capitalCurrentAccount.BusinessName ?? (object)DBNull.Value),
                        new MySqlParameter("@CurrentAccountBalance", capitalCurrentAccount.CurrentAccountBalance ?? (object)DBNull.Value),
                        new MySqlParameter("@CapitalAccountBalance", capitalCurrentAccount.CapitalAccountBalance ?? (object)DBNull.Value),
                         new MySqlParameter("@transactionType", capitalCurrentAccount.TransactionType ?? (object)DBNull.Value),
                        new MySqlParameter("@SelfonlineBusinessAccountID", capitalCurrentAccount.SelfonlineBusinessAccountID)

              );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveEditSelfonlineAssetsCapitalCurrentAccount");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineAssetsCapitalCurrentAccountDto>> GetSelfOnlineAssetCapitalCurrentAccount(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineAssetsCapitalCurrentAccountDto> assetsCapitalCurrentAccountList = [];
        try
        {

            assetsCapitalCurrentAccountList = await _context.SelfonlineAssetsCapitalCurrentAccount
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineAssetsCapitalCurrentAccountDto
            {
                SelfonlineBusinessAccountID = t.SelfonlineBusinessAccountID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                BusinessName = t.BusinessName,
                CurrentAccountBalance = t.CurrentAccountBalance,
                CapitalAccountBalance = t.CapitalAccountBalance

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return assetsCapitalCurrentAccountList;
    }

    //-------- Liabilities

    public async Task<bool> SaveEditSelfonlineLiabilitiesAllLiabilities(SelfonlineLiabilitiesAllLiabilitiesDto allLiabilities)
    {
        bool isSuccess = false;
        try
        {
        

                await _context.Database.ExecuteSqlRawAsync(
              @"CALL ADDEditSelfOnlineLiability  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @Description,
                                        @SecurityOnLiability,
                                        @DateOfCommencement,
                                        @OriginalAmount,
                                        @AmountAsAt,
                                        @AmountRepaid,
                                        @transactionType,
                                        @SelfonlineLiabilityID
                                    )",
                     new MySqlParameter("@loguser", allLiabilities.UserId ?? (object)DBNull.Value),
                    new MySqlParameter("@UserId", allLiabilities.UserId ?? (object)DBNull.Value),
                    new MySqlParameter("@Year", allLiabilities.Year),
                    new MySqlParameter("@Type", allLiabilities.Type ?? (object)DBNull.Value),
                    new MySqlParameter("@SerialNumber", allLiabilities.SerialNumber ?? (object)DBNull.Value),
                    new MySqlParameter("@Description", allLiabilities.Description ?? (object)DBNull.Value),
                    new MySqlParameter("@SecurityOnLiability", allLiabilities.SecurityOnLiability ?? (object)DBNull.Value),
                    new MySqlParameter("@DateOfCommencement", allLiabilities.DateOfCommencement ?? (object)DBNull.Value),
                    new MySqlParameter("@OriginalAmount", allLiabilities.OriginalAmount ?? (object)DBNull.Value),
                    new MySqlParameter("@AmountAsAt", allLiabilities.AmountAsAt ?? (object)DBNull.Value),
                    new MySqlParameter("@AmountRepaid", allLiabilities.AmountRepaid ?? (object)DBNull.Value),
                     new MySqlParameter("@transactionType", allLiabilities.TransactionType ?? (object)DBNull.Value),
                    new MySqlParameter("@SelfonlineLiabilityID", allLiabilities.SelfonlineLiabilityID)

          );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveEditSelfonlineAssetsCapitalCurrentAccount");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineLiabilitiesAllLiabilitiesDto>> GetSelfonlineLiabilitiesAllLiabilities(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineLiabilitiesAllLiabilitiesDto> assetsCapitalCurrentAccountList = [];
        try
        {

            assetsCapitalCurrentAccountList = await _context.SelfonlineLiabilitiesAllLiabilities
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineLiabilitiesAllLiabilitiesDto
            {
                SelfonlineLiabilityID = t.SelfonlineLiabilityID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                Description = t.Description,
                SecurityOnLiability = t.SecurityOnLiability,
                DateOfCommencement = t.DateOfCommencement,
                OriginalAmount = t.OriginalAmount,
                AmountAsAt = t.AmountAsAt,
                AmountRepaid = t.AmountRepaid,

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return assetsCapitalCurrentAccountList;
    }

    public async Task<bool> SaveEditSelfonlineLiabilitiesOtherAssetsGifts(SelfonlineLiabilitiesOtherAssetsGiftsDto otherAssetss)
    {
        bool isSuccess = false;
        try
        {

            

                await _context.Database.ExecuteSqlRawAsync(
          @"CALL ADDEditSelfOnlineAssetsGifts  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @Description,
                                        @AcquisitionMode,
                                        @DateOfAcquisition,
                                        @CostMarketValue,
                                        @transactionType,
                                        @selfonlineAssetsGiftsID
                                    )",
                 new MySqlParameter("@loguser", otherAssetss.UserId ?? (object)DBNull.Value),
                new MySqlParameter("@UserId", otherAssetss.UserId ?? (object)DBNull.Value),
                new MySqlParameter("@Year", otherAssetss.Year),
                new MySqlParameter("@Type", otherAssetss.Type ?? (object)DBNull.Value),
                new MySqlParameter("@SerialNumber", otherAssetss.SerialNumber ?? (object)DBNull.Value),
                new MySqlParameter("@Description", otherAssetss.Description ?? (object)DBNull.Value),
                new MySqlParameter("@AcquisitionMode", otherAssetss.AcquisitionMode ?? (object)DBNull.Value),
                new MySqlParameter("@DateOfAcquisition", otherAssetss.DateOfAcquisition ?? (object)DBNull.Value),
                new MySqlParameter("@CostMarketValue", otherAssetss.CostMarketValue ?? (object)DBNull.Value),
                 new MySqlParameter("@transactionType", otherAssetss.TransactionType ?? (object)DBNull.Value),
                new MySqlParameter("@selfonlineAssetsGiftsID", otherAssetss.SelfonlineAssetsGiftsID)

      );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveEditSelfonlineLiabilitiesOtherAssetsGifts");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineLiabilitiesOtherAssetsGiftsDto>> GetSelfonlineLiabilitiesOtherAssetsGifts(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineLiabilitiesOtherAssetsGiftsDto> assetsCapitalCurrentAccountList = [];
        try
        {

            assetsCapitalCurrentAccountList = await _context.SelfonlineLiabilitiesOtherAssetsGifts
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineLiabilitiesOtherAssetsGiftsDto
            {
                SelfonlineAssetsGiftsID = t.SelfonlineAssetsGiftsID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                Description = t.Description,
                AcquisitionMode = t.AcquisitionMode,
                DateOfAcquisition = t.DateOfAcquisition,
                CostMarketValue = t.CostMarketValue

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return assetsCapitalCurrentAccountList;
    }

    public async Task<bool> SaveEditSelfonlineLiabilitiesDisposalAssets(SelfonlineLiabilitiesDisposalAssetsDto disposalAssets)
    {
        bool isSuccess = false;
        try
        {

           

                await _context.Database.ExecuteSqlRawAsync(
      @"CALL ADDEditSelfOnlineCapitalCurrentAccount  (
                                        @loguser,
                                        @UserId,
                                        @Year,
                                        @Type,
                                        @SerialNumber,
                                        @Description,
                                        @DateOfDisposal,
                                        @SalesProceed,
                                        @DateAcquired,
                                        @Cost,
                                        @transactionType,
                                        @selfonlineDisposalAssetsID
                                    )",
             new MySqlParameter("@loguser", disposalAssets.UserId ?? (object)DBNull.Value),
            new MySqlParameter("@UserId", disposalAssets.UserId ?? (object)DBNull.Value),
            new MySqlParameter("@Year", disposalAssets.Year),
            new MySqlParameter("@Type", disposalAssets.Type ?? (object)DBNull.Value),
            new MySqlParameter("@SerialNumber", disposalAssets.SerialNumber ?? (object)DBNull.Value),
            new MySqlParameter("@Description", disposalAssets.Description ?? (object)DBNull.Value),
            new MySqlParameter("@DateOfDisposal", disposalAssets.DateOfDisposal ?? (object)DBNull.Value),
            new MySqlParameter("@SalesProceed", disposalAssets.SalesProceed ?? (object)DBNull.Value),
            new MySqlParameter("@DateAcquired", disposalAssets.DateAcquired ?? (object)DBNull.Value),
            new MySqlParameter("@Cost", disposalAssets.Cost ?? (object)DBNull.Value),
             new MySqlParameter("@transactionType", disposalAssets.TransactionType ?? (object)DBNull.Value),
            new MySqlParameter("@selfonlineDisposalAssetsID", disposalAssets.SelfonlineDisposalAssetsID)

  );

            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SaveEditSelfonlineLiabilitiesOtherAssetsGifts");
        }

        return isSuccess;
    }
    public async Task<List<SelfonlineLiabilitiesDisposalAssetsDto>> GetSelfonlineLiabilitiesDisposalAssets(string userId, int year, CancellationToken ctx)
    {
        List<SelfonlineLiabilitiesDisposalAssetsDto> assetsCapitalCurrentAccountList = [];
        try
        {

            assetsCapitalCurrentAccountList = await _context.SelfonlineLiabilitiesDisposalAssets
            .Where(b => b.UserId == userId && b.Year == year)
            .Select(t => new SelfonlineLiabilitiesDisposalAssetsDto
            {
                SelfonlineDisposalAssetsID = t.SelfonlineDisposalAssetsID,
                UserId = t.UserId,
                Year = t.Year,
                Type = t.Type,
                SerialNumber = t.SerialNumber,
                Description = t.Description,
                DateOfDisposal = t.DateOfDisposal,
                SalesProceed = t.SalesProceed,
                DateAcquired = t.DateAcquired,
                Cost = t.Cost

            })
            .ToListAsync(ctx);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return assetsCapitalCurrentAccountList;
    }

}


