using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase, IDisposable
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        if (await _userService.EmailExists(createUserDto.Email))
            return BadRequest(new ValidationError($"Email {createUserDto.Email} is already exists."));
        string token = await _userService.Register(createUserDto);
        return Created("", token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        string? token = await _userService.Login(loginDto);
        if (token == null) return Unauthorized(new UnauthorizedError("Incorrect email or password"));
        return Ok(token);
    }

    public void Dispose()
    {
        _userService.Dispose();
    }
}
