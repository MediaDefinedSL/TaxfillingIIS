using TaxFiling.API.Services;
using TaxFiling.Business.Interfaces;
using TaxFiling.Business.Repositories;
using TaxFiling.Business.Services;

namespace TaxFiling.API.Extentions;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();

        services.AddTransient<IEmailService, EmailService>();

        services.AddTransient<IUserRepository, UserRepository>();

        services.AddTransient<IPackagesRepository, PackagesRepository>();

        services.AddTransient<ISelfOnlineFlowRepository, SelfOnlineFlowRepository>();

        services.AddTransient<IUserUploadTaxAssistedDocRepository, UserUploadTaxAssistedDocRepository>();
        services.AddTransient<IUserTaxAssistedOtherAssetsDetailsRepository, UserTaxAssistedOtherAssetsDetailsRepository>();


    }
}
