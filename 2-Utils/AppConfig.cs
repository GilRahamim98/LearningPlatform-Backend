namespace Talent;

public class AppConfig
{
    // Indicates if the application is running in production environment
    public static bool IsProduction;
    public static string ConnectionString { get; private set; } = null!;
    public static string JwtKey { get; private set; } = "pL5Vpu8DYG3NCZwy4q6RdEjTMvH7xF2bKskAnWgQ9JXZfUDasdopuxcPajkl489489dniONJKSADBUInbcNJSHIU";
    public static int JwtKeyExpire { get; private set; }

    // Configures application settings based on the environment
    public static void Configure(IWebHostEnvironment env)
    {
        IsProduction = env.IsProduction();

        // Build configuration settings from appsettings.json and environment-specific appsettings file
        IConfigurationRoot settings = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{env.EnvironmentName}.json").Build();
        
        ConnectionString = settings.GetConnectionString("AcademiaX")!;
        JwtKeyExpire = env.IsDevelopment() ? 24 : 8;
    }
}
