using FinanceTracker.Domain.Aggregates;
using FinanceTracker.Infrastructure.Brokers.Email;
using FinanceTracker.Infrastructure.Brokers.Logging;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Infrastructure.Providers.AuthProvider;
using FinanceTracker.Infrastructure.Providers.FileProvider;
using FinanceTracker.Services.Foundations;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations;
using FinanceTracker.Services.Orchestrations.Interfaces;
using FinanceTracker.Services.Processings;

namespace FinanceTracker.Presentation.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<ICategoryOrchestrationService, CategoryOrchestrationService>();
            services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailBroker, EmailBroker>();
            services.AddHttpContextAccessor();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IStorageBroker, StorageBroker>();
            services.AddSingleton<ILoggingBroker, LoggingBroker>();

            services.AddScoped<TransactionAggregate>();
            services.AddScoped<AccountAggregate>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<IAuthProvider, AuthProvider>();

            services.AddScoped<ITransactionOrchestration, TransactionOrchestration>();
            services.AddScoped<IUserOrchestration, UserOrchestration>();
            services.AddScoped<IReportOrchestrationService, ReportOrchestrationService>();

            services.AddScoped<IDashboardProcessingService, DashboardProcessingService>();
            return services;
        }
    }
}
