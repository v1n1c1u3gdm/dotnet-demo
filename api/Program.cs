using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
        .AddCors()
        .AddDbContext<DataContext>(opt=>
            opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
        ).AddControllers();

var app = builder.Build();

app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod())
    .UseHttpsRedirection();
app.MapControllers();

app.Run();