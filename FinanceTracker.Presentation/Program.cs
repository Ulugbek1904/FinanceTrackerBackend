using FinanceTracker.Infrastructure.Brokers.Storages;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Foundations;
using FinanceTracker.Services.Orchestrations.Interfaces;
using FinanceTracker.Services.Orchestrations;
using FinanceTracker.Domain.Aggregates;


namespace FinanceTracker.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<StorageBroker>(options =>
            {
                var connectionString = builder.
                Configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connectionString); 
            });

            builder.Services.AddScoped<IStorageBroker>(provider => provider.GetRequiredService<StorageBroker>());

            // Program.cs
            builder.Services.AddScoped<TransactionAggregate>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITransactionOrchestration, TransactionOrchestration>();
            builder.Services.AddScoped<IUserOrchestration, UserOrchestration>();
            builder.Services.AddScoped<IReportOrchestrationService, ReportOrchestrationService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();    
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

    }
}
