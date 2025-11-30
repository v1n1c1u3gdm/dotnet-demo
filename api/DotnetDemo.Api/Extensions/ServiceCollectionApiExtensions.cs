using System;
using System.Text;
using System.Text.Json;
using DotnetDemo.Configuration;
using DotnetDemo.Domain.Requests.Validators;
using DotnetDemo.Security;
using DotnetDemo.Services;
using DotnetDemo.Tech;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace DotnetDemo.Extensions;

public static class ServiceCollectionApiExtensions
{
    public static IServiceCollection AddApiCore(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "dotnet-demo API",
                Version = "v1",
                Description = "ASP.NET Core + NHibernate reimplementation of the ruby-demo API."
            });
        });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<ArticleRequestValidator>();

        services.AddCors(options =>
        {
            options.AddPolicy("default", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ISocialService, SocialService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ITechReportService, TechReportService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services
            .AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), "Jwt:SecretKey must be configured.")
            .ValidateOnStart();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
            new ConfigureNamedOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var jwtOptions = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
                    if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
                    {
                        throw new InvalidOperationException("Jwt:SecretKey must be configured.");
                    }

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                }));

        services.AddAuthorization();

        return services;
    }
}

