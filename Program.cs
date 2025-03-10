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
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console() 
                        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) 
                        .Enrich.FromLogContext()
                        .MinimumLevel.Information() 
                        .CreateLogger();

        AppConfig.Configure(builder.Environment);

        if (builder.Environment.IsProduction())
        {
            using (AcademiaXContext db = new AcademiaXContext())
            {
                db.Database.EnsureCreated();
            }
        }

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularOrigin",
                builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        builder.Services.AddDbContext<AcademiaXContext>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<LessonService>();
        builder.Services.AddScoped<EnrollmentService>();
        builder.Services.AddScoped<ProgressService>();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CourseValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LessonValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<EnrollmentValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ProgressValidator>();
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddMvc(options => options.Filters.Add<CatchAllFilter>()); 

        builder.Host.UseSerilog();


        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtHelper.SetBearerOptions);

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
