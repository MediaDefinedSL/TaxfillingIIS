using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxFiling.Business.Interfaces;
using TaxFiling.Data;
using TaxFiling.Domain.Dtos;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Business.Repositories;

public class PackagesRepository  :  IPackagesRepository

{
    private readonly Context _context;
    private readonly ILogger<PackagesRepository> _logger;
    private readonly IConfiguration _configuration;
    public int Result { get; set; }
    public Guid ResultGuid { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public PackagesRepository(Context context, ILogger<PackagesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<List<PackagesDto>> GetPackages(int IsSelfFiling ,CancellationToken cancellationToken)
    {
        List<PackagesDto> packageses = [];
        try
        {
            packageses = await _context.Packages
                .Where(u => u.IsSelfFiling == IsSelfFiling)
                .Select(u => new PackagesDto
                {
                    PackagesId = u.PackagesId,
                    Name = u.Name,
                    Description = u.Description,
                    Curancy = u.Curancy,
                    Price = u.Price,
                    ImageUrl = u.ImageUrl
                    
                }).ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        return packageses;
    }

    public async Task<PackagesDto?> GetPackageDetails(int id, CancellationToken ctx)
    {
        var package = await _context.Packages
                            .AsNoTracking()
                            .Where(p => p.PackagesId == id)
                            .Select(p => new PackagesDto
                            {
                                PackagesId = p.PackagesId,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                Curancy = p.Curancy
                            })
                            .FirstOrDefaultAsync(ctx);

        

        return package;
    }

}
