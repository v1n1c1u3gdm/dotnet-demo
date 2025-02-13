using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt=>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"))
);

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();