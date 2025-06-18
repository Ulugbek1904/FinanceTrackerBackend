using System.Text;
using FluentValidation;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinanceTracker.Presentation.Mappings;
using FinanceTracker.Presentation.Extensions;
using FinanceTracker.Presentation.Middleware;
using FinanceTracker.Presentation.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Infrastructure.Brokers.Storages.Seed;
using System.Net;
using Microsoft.Extensions.FileProviders;


namespace FinanceTracker.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://+:8080");
            // Db Context ------------>
            builder.Services.AddDbContext<StorageBroker>(options =>
            {
                options.UseNpgsql(builder.Configuration
                    .GetConnectionString("DefaultConnection"));
            });

            // Fluent Validation and CORS -------------->
            builder.Services.AddValidatorsFromAssemblyContaining
                <TransactionQueryValidator>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", policy =>
                {
                    policy.WithOrigins("https://finance-tracker-latest-coiu.onrender.com", "http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Rate Limiting ---------------->
            builder.Services.AddMemoryCache();

            builder.Services.Configure<IpRateLimitOptions>
                (builder.Configuration.GetSection("IpRateLimiting"));

            builder.Services.AddSingleton<IIpPolicyStore,
                MemoryCacheIpPolicyStore>();

            builder.Services.AddSingleton<IRateLimitCounterStore, 
                MemoryCacheRateLimitCounterStore>();

            builder.Services.AddSingleton<IRateLimitConfiguration,
                RateLimitConfiguration>();

            builder.Services.AddSingleton<IProcessingStrategy,
                AsyncKeyLockProcessingStrategy>();

            // AutoMapper Custom service extension ----------------->
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddApplicationService();

            // Controllers & Validation Error Handling----------------------->
            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(x => 
                                x.ErrorMessage).ToArray()
                        );

                    var problemDetails = new ValidationProblemDetails(errors)
                    {
                        Title = "Invalid Model",
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400"
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });

            // Swagger ------------------------------------>
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FinanceTracker API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT authorization using " +
                        "Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[] {}
                    }
                });
            });

            //Authentication & Authorization ------------------------>
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseMiddleware<ProblemDetailsMiddleware>();
            app.UseRouting();

            var storagePath = "/app/LocalFileStorage";
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(storagePath),
                RequestPath = "/profile-pictures"
            });

            app.UseCors("AllowAngularClient");

            // Super Admin seed
            await AppDbInitializer.SeedSuperAdminAsync(app.Services);

            // Swagger
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Security
            app.UseHttpsRedirection(); 
            app.UseAuthentication();
            app.UseAuthorization();

            // Rate Limiting
            app.UseIpRateLimiting();

            // Token Validation Middleware
            app.UseMiddleware<TokenValidationMiddleware>();

            // Controllers
            app.MapControllers();

            app.Run();
        }
    }
}