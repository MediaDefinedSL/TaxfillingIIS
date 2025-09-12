using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Diagnostics.Metrics;
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

}


