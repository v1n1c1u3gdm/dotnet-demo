using System;
using System.Text;
using System.Text.Json;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Extensions;

public static class ServiceMappingExtension
{
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenService, TokenService>();
        return builder;
    }

    public static WebApplicationBuilder ConfigureFrameworkServices(this WebApplicationBuilder builder)
    {
        builder.Services
                .AddCors()
                .AddDbContext<DataContext>(opt =>
                    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
                ).AddControllers().AddJsonOptions(opts => {
                    opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    opts.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
                });

        builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    var tokenKey = GetTokenKey(builder);
                    opts.TokenValidationParameters = ConfigureValidationParameters(tokenKey);
                });

        return builder;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
            .UseHttpsRedirection();

        app.UseAuthentication().UseAuthorization();

        app.MapControllers();

        return app;
    }

    private static TokenValidationParameters ConfigureValidationParameters(string tokenKey) =>
        new()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey))
        };
        
    private static string GetTokenKey(WebApplicationBuilder builder) =>
        builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey property not configured");
}
