using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    public async Task<List<TaxPayerDto>> GetTaxPayers(CancellationToken cancellationToken)
    {
        List<TaxPayerDto> taxPayers = [];
        try
        {

            taxPayers = await _context.TaxPayers
                .Select(t => new TaxPayerDto
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

        return taxPayers;
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
                                            CareOf = b.CareOf,
                                            Apt = b.Apt,
                                            StreetNumber = b.StreetNumber,
                                            Street = b.Street,
                                            City = b.City

                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(ctx);
        return bookingViewModel;

    }

    public async Task<bool> UpdateTaxPayer(string userId, int year, int taxPayerId)
    {
        bool isSuccess = false;


        try
        {
            var _user = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == userId && p.Year == year)
                                .FirstOrDefaultAsync();

            _user.TaxpayerId = taxPayerId;

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

    public async Task<bool> UpdatelIdentification(string userId, int year, string firstName, string middleName, string lastName, DateTime dateofbirth, string taxnumber)
    {
        bool isSuccess = false;


        try
        {
            var _selfOnlineuser = await _context.SelfOnlineFlowPersonalInformation
                                .Where(p => p.UserId == userId && p.Year == year)
                                .FirstOrDefaultAsync();

            if (_selfOnlineuser != null)
            {
                _selfOnlineuser.FirstName = firstName;
                _selfOnlineuser.MiddleName = middleName;
                _selfOnlineuser.LastName = lastName;
                _selfOnlineuser.DateOfBirth = dateofbirth;
                _selfOnlineuser.TaxNumber = taxnumber;

                await _context.SaveChangesAsync();
            }

            var _user = await _context.Users
                .Where(p => p.UserId.ToString() == userId)
                .FirstOrDefaultAsync();

            if (_user != null)
            {
                _user.FirstName = firstName;
                _user.LastName = lastName;
                _user.TinNo = taxnumber;

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

}
   
