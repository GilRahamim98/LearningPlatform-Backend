using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
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

        builder.Services.AddDbContext<AcademiaXContext>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<LessonService>();
        builder.Services.AddScoped<EnrollmentService>();
        builder.Services.AddScoped<ProgressService>();
        builder.Services.AddScoped<IValidator<RegisterUserDto>,RegisterValidator>();
        builder.Services.AddScoped<IValidator<LoginUserDto>,LoginValidator>();
        builder.Services.AddScoped<IValidator<CreateCourseDto>,CourseValidator>();
        builder.Services.AddScoped<IValidator<CreateLessonDto>,LessonValidator>();
        builder.Services.AddScoped<IValidator<CreateEnrollmentDto>,EnrollmentValidator>();
        builder.Services.AddScoped<IValidator<CreateProgressDto>,ProgressValidator>();
        builder.Services.AddAutoMapper(typeof(Program));
        
        builder.Host.UseSerilog();


        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtHelper.SetBearerOptions);

        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

        var app = builder.Build();


        app.UseSerilogRequestLogging();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
