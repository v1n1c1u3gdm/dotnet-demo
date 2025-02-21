using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureFrameworkServices(builder);
            ConfigureApplicationServices(builder);

            var app = builder.Build();
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ITokenService, TokenService>();
        }

        private static void ConfigureFrameworkServices(WebApplicationBuilder builder)
        {
            builder.Services
                    .AddCors()
                    .AddDbContext<DataContext>(opt =>
                        opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
                    ).AddControllers();

            builder.Services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts =>
                    {
                        var tokenKey = GetTokenKey(builder);
                        opts.TokenValidationParameters = ConfigureValidationParameters(tokenKey);
                    });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                .UseHttpsRedirection();

            app.UseAuthentication().UseAuthorization();

            app.MapControllers();
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
}