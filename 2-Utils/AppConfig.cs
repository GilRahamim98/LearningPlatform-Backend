namespace Talent;

public class AppConfig
{
    public static bool IsProduction;
    public static string ConnectionString { get; private set; } = null!;
    public static string JwtKey { get; private set; } = "pL5Vpu8DYG3NCZwy4q6RdEjTMvH7xF2bKskAnWgQ9JXZfU";
    public static int JwtKeyExpire { get; private set; }

    public static void Configure(IWebHostEnvironment env)
    {
        IsProduction = env.IsProduction();
        IConfigurationRoot settings = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{env.EnvironmentName}.json").Build();
        ConnectionString = settings.GetConnectionString("AcademiaX")!;

        JwtKeyExpire = env.IsDevelopment() ? 5 : 1;
    }
}
