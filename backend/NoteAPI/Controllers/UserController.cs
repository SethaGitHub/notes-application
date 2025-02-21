using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NoteAPI.Models;
using NoteAPI.Services;
using NoteAPI.DTOs;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);
        return user is not null ? Ok(user) : NotFound();
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email);
        return user is not null ? Ok(user) : NotFound();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _userService.RegisterUser(registerDto);
        return result ? Ok(new { message = "User registered successfully" }) : BadRequest("User already exists");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _userService.LoginUser(loginDto);
        return token != null ? Ok(new { token }) : Unauthorized("Invalid email or password");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteUser(id);
        return deleted ? NoContent() : NotFound();
    }
}
