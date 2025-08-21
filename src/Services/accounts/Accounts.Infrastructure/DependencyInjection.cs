using Accounts.Application.Interfaces;
using Accounts.Infrastructure.Data;
using Accounts.Infrastructure.Repositories;
using Accounts.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<AccountsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AccountsDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
            
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching(true);
            options.EnableDetailedErrors(false);
        });

        // Repositories
        services.AddScoped(typeof(IAccountsRepository<>), typeof(AccountsRepository<>));
        services.AddScoped<IChartOfAccountsRepository, ChartOfAccountsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<ICashbookRepository, CashbookRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Unit of Work
        services.AddScoped<IAccountsUnitOfWork, AccountsUnitOfWork>();

        // Services
        services.AddScoped<IPostingRulesService, PostingRulesService>();

        return services;
    }
}
