using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase, IDisposable
{
    private readonly UserService _userService;
    private readonly EnrollmentService _enrollmentService;
    private readonly ProgressService _progressService;
    private readonly CourseService _courseService;

    private IValidator<RegisterUserDto> _registerValidator;
    private IValidator<LoginUserDto> _loginValidator;
    private IValidator<CreateEnrollmentDto> _enrollmentValidator;
    private IValidator<CreateProgressDto> _progressValidator;

    public UserController(
        UserService userService,
        EnrollmentService enrollmentService,
        ProgressService progressService,
        CourseService courseService,
         IValidator<RegisterUserDto> registerValidator,
         IValidator<LoginUserDto> loginValidator,
         IValidator<CreateEnrollmentDto> enrollmentValidator,
         IValidator<CreateProgressDto> progressValidator)
    {
        _userService = userService;
        _enrollmentService = enrollmentService;
        _progressService = progressService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _enrollmentValidator = enrollmentValidator;
        _courseService = courseService;
        _progressValidator = progressValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto createUserDto)
    {
        ValidationResult validationResult = _registerValidator.Validate(createUserDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (await _userService.EmailExists(createUserDto.Email))
            errors.Add("Email is already exists.");

        if (errors.Any())
            return BadRequest(new ValidationError<List<string>>(errors));

        string token = await _userService.Register(createUserDto);
        return Created("", token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        ValidationResult validationResult = _loginValidator.Validate(loginDto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ValidationError<List<string>>(errors));
        }

        string? token = await _userService.Login(loginDto);
        if (token == null) return Unauthorized(new UnauthorizedError("Incorrect email or password"));
        return Ok(token);
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpPost("enroll-to-course")]
    public async Task<IActionResult> EnrollUser([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        ValidationResult validationResult = _enrollmentValidator.Validate(createEnrollmentDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (!await _courseService.CourseExists(createEnrollmentDto.CourseId)) errors.Add("Course not found");

        if (!await _userService.UserExists(createEnrollmentDto.UserId)) errors.Add("User not found");

        bool alreadyEnrolled = await _enrollmentService.IsUserEnrolled(createEnrollmentDto.UserId, createEnrollmentDto.CourseId);
        if (alreadyEnrolled) errors.Add("User is already enrolled in this course.");

        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        EnrollmentDto newEnrollment = await _enrollmentService.EnrollUserInCourse(createEnrollmentDto);
        return Created("/api/users/enroll-to-course", newEnrollment);
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpGet("enrollments/{userId}")]
    public async Task<IActionResult> GetUserEnrollments([FromRoute] Guid userId)
    {
        List<CourseDto> courses = await _enrollmentService.GetUserEnrollments(userId);
        return Ok(courses);
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpDelete("unenroll")]
    public async Task<IActionResult> UnenrollUser([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        bool deleted = await _enrollmentService.UnenrollUserFromCourse(createEnrollmentDto);
        if (!deleted) return NotFound(new ResourceNotFound($"Enrollment not found for this user id : {createEnrollmentDto.UserId} and for this course id : {createEnrollmentDto.CourseId}"));
        return NoContent();
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpGet("progress-by-user/{userId}")]
    public async Task<IActionResult> GetProgressByUser([FromRoute] Guid userId)
    {
        List<ProgressDto> progresses = await _progressService.GetProgressByUser(userId);
        return Ok(progresses);
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpPost("progresses")]
    public async Task<IActionResult> AddProgress([FromBody] CreateProgressDto createProgressDto)
    {
        ValidationResult validationResult = _progressValidator.Validate(createProgressDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (!await _progressService.IsUserEnrolled(createProgressDto)) errors.Add("The user is not enrolled for this lesson's course.");
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        ProgressDto dbProgress = await _progressService.AddProgress(createProgressDto);
        return Created("api/progresses/" + dbProgress.Id, dbProgress);
    }

    [Authorize(Roles = "Admin,Student")]
    [HttpPut("progresses/{id}")]
    public async Task<IActionResult> UpdateProgress([FromRoute] Guid id, [FromBody] CreateProgressDto createProgressDto)
    {
        ValidationResult validationResult = _progressValidator.Validate(createProgressDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (!await _progressService.IsUserEnrolled(createProgressDto)) errors.Add("The user is not enrolled for this lesson's course.");
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        ProgressDto? dbProgress = await _progressService.UpdateProgress(id, createProgressDto);
        return Ok(dbProgress);
    }

    public void Dispose()
    {
        _userService.Dispose();
        _enrollmentService.Dispose();
        _progressService.Dispose();
        _courseService.Dispose();
    }
}
