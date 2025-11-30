using DotnetDemo;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureLogging(builder);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

await startup.ApplyDatabaseMigrationsAsync(app.Services);
startup.ConfigurePipeline(app);

await app.RunAsync();

public partial class Program;
