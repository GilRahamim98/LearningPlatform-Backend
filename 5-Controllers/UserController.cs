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
            return BadRequest("User not valid.");
        if (_userService.EmailExists(user.Email))
            return BadRequest("Email is already exists.");
        string token = _userService.Register(user);
        return Created("", token);
    }

    public void Dispose()
    {
        _userService.Dispose();
    }
}
