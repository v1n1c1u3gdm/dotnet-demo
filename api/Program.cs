using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Mapping framework services
builder.Services
        .AddCors()
        .AddDbContext<DataContext>(opt=>
            opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
        ).AddControllers();

//Application services
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

//Configuring framework
app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
    .UseHttpsRedirection();
app.MapControllers();

app.Run();