using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase, IDisposable
{
    private readonly UserService _userService;
    private readonly EnrollmentService _enrollmentService;
    private readonly ProgressService _progressService;

    private IValidator<RegisterUserDTO> _registerValidator;
    private IValidator<LoginUserDto> _loginValidator;
    private IValidator<CreateEnrollmentDto> _enrollmentValidator;
    public UserController(
        UserService userService,
        EnrollmentService enrollmentService,
        ProgressService progressService,
         IValidator<RegisterUserDTO> registerValidator,
         IValidator<LoginUserDto> loginValidator,
         IValidator<CreateEnrollmentDto> enrollmentValidator)
    {
        _userService = userService;
        _enrollmentService = enrollmentService;
        _progressService = progressService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _enrollmentValidator = enrollmentValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterUserDTO createUserDto)
    {
        ValidationResult validationResult = _registerValidator.Validate(createUserDto);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationError(ModelState.GetAllErrors()));
       
        string token = await _userService.Register(createUserDto);
        return Created("", token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
    {
        ValidationResult validationResult = _loginValidator.Validate(loginDto);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        string? token = await _userService.Login(loginDto);
        if (token == null) return Unauthorized(new UnauthorizedError("Incorrect email or password"));
        return Ok(token);
    }

    public void Dispose()
    {
        _userService.Dispose();
    }
}
