using Microsoft.EntityFrameworkCore;
using TaxFiling.Domain.Entities;

namespace TaxFiling.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {

    }

    #region USER ADMIN...

    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    #endregion
    public DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<EmailSetting> EmailSettings { get; set; }

    public DbSet<Packages> Packages { get; set; }
    public DbSet<TaxPayer> TaxPayers { get; set; }
    public DbSet<TaxReturnLastyear> TaxReturnLastyears { get; set; }
 
    public DbSet<MaritalStatus> MaritalStatuses { get; set; }

    public DbSet<SelfOnlineFlowPersonalInformation> SelfOnlineFlowPersonalInformation { get; set; }
    public DbSet<UserUploadTaxAssistedDoc> UserUploadTaxAssistedDocs { get; set; }

}
