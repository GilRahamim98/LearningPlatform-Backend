using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;

namespace Talent;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a new WebApplication builder with the provided arguments
        var builder = WebApplication.CreateBuilder(args);

        // Register the background service
        builder.Services.AddHostedService<LogCleanupService>();

        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console() 
                        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
                        .Enrich.FromLogContext()
                        .MinimumLevel.Information() 
                        .CreateLogger();

        // Configure application settings based on the environment
        AppConfig.Configure(builder.Environment);

        // Ensure the database is created in production environment
        if (builder.Environment.IsProduction())
        {
            using (AcademiaXContext db = new AcademiaXContext())
            {
                db.Database.EnsureCreated();
            }
        }

        // Configure CORS to allow requests from Angular application
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularOrigin",
                builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        // Register application services and dependencies
        builder.Services.AddDbContext<AcademiaXContext>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<LessonService>();
        builder.Services.AddScoped<EnrollmentService>();
        builder.Services.AddScoped<ProgressService>();

        // Register FluentValidation validators
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CourseValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LessonValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LessonDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ProgressValidator>();

        // Register AutoMapper profiles
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // Add MVC with a global exception filter
        builder.Services.AddMvc(options => options.Filters.Add<CatchAllFilter>());

        // Use Serilog for logging
        builder.Host.UseSerilog();

        // Configure API behavior options
        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        // Configure JWT authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtHelper.SetBearerOptions);

        // Configure JSON options for controllers
        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        app.UseAuthorization();

        app.UseCors("AllowAngularOrigin");
        app.MapControllers();

        app.Run();
    }
}
