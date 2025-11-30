using System.Text.Json;
using DotnetDemo.Domain.Requests.Validators;
using DotnetDemo.Services;
using DotnetDemo.Tech;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DotnetDemo.Extensions;

public static class ServiceCollectionApiExtensions
{
    public static IServiceCollection AddApiCore(this IServiceCollection services)
    {
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
        services.AddSingleton<ITechReportService, TechReportService>();

        return services;
    }
}

