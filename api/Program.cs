using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt=>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
);

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200, https://localhost:4200"));
app.MapControllers();
app.Run();