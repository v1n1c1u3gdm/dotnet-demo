namespace DotnetDemo.Seeding;

public class SeedAdminOptions
{
    public const string SectionName = "Seeds:AdminUser";

    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin123456";
    public string DisplayName { get; set; } = "Administrador";
    public int HashIterations { get; set; } = 120_000;
}

