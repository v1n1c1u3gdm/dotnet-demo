using api.Extensions;


var builder = WebApplication.CreateBuilder(args)
    .ConfigureFrameworkServices()
    .ConfigureApplicationServices();

var app = builder.Build();
app.ConfigureMiddleware();
app.Run();
