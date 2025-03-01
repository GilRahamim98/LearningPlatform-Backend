using Microsoft.AspNetCore.Mvc;

namespace Talent;

[ApiController]
public class UserController : ControllerBase, IDisposable
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("api/register")]
    public IActionResult Register([FromBody]User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        if (_userService.EmailExists(user.Email))
            return BadRequest(new ValidationError($"Email {user.Email} is already exists."));
        string token = _userService.Register(user);
        return Created("", token);
    }

    [HttpPost("api/login")]
    public IActionResult Login([FromBody] Credentials credentials)
    {
        string? token = _userService.Login(credentials);
        if (token == null) return Unauthorized(new UnauthorizedError("Incorrect email or password"));
        return Ok(token);
    }

    public void Dispose()
    {
        _userService.Dispose();
    }
}
