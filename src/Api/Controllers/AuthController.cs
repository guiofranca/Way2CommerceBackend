using Api.Service;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mysql.Identity;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository<ApplicationUser> _userRepository;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController([FromServices] IUserRepository<ApplicationUser> userRepository, [FromServices] JwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        ApplicationUser? user = await _userRepository.TryLoginAsync(loginRequest.Email, loginRequest.Password);
        if (user == null) return BadRequest("Invalid email and/or password.");
        
        IEnumerable<string>? roles = await _userRepository.GetUserRolesAsync(user);
        string token = _jwtTokenService.GenerateToken(user, roles);
        string refreshToken = await _jwtTokenService.GenerateRefreshToken(user);

        return Ok(new LoginResponse(token, refreshToken));
    }

    [HttpPost("Logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> Logout()
    {
        return NoContent();
    }

    [HttpPost("Register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest registerRequest)
    {
        ApplicationUser? user = await _userRepository.RegisterAsync(registerRequest.Name, registerRequest.Email, registerRequest.Password);

        if (user == null) return BadRequest("Could not create user");

        IEnumerable<string> roles = new string[] { };

        if(await _userRepository.AddRole(user, "User")) roles = new string[] { "User" };

        string token = _jwtTokenService.GenerateToken(user, roles);
        string refreshToken = await _jwtTokenService.GenerateRefreshToken(user);
        return Created("", new RegisterResponse(registerRequest.Name, registerRequest.Email, token, refreshToken));
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest refreshTokenRequest, [FromHeader] string Authorization)
    {
        string userId = _jwtTokenService.GetUserIdFromExpiredToken(Authorization.Replace("Bearer ", ""));
        ApplicationUser? user = await _userRepository.GetByIdAsync(userId);

        if (user == null) return NotFound();
        
        string refreshToken = await _jwtTokenService.GenerateNewTokenBasedOnRefreshToken(user, refreshTokenRequest.RefreshToken);
        IEnumerable<string>? roles = await _userRepository.GetUserRolesAsync(user);
        string token = _jwtTokenService.GenerateToken(user, roles);
        return Ok(new RefreshTokenResponse(token, refreshToken));
    }

    [HttpPost("AddRole")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<AddRoleResponse>> AddRole(AddRoleRequest addRoleRequest)
    {
        ApplicationUser? user = await _userRepository.GetByIdAsync(User.Claims.ToList().Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
        if (user == null) return NotFound();

        bool success = await _userRepository.AddRole(user, addRoleRequest.Name);
        
        if (!success) return BadRequest("Could not add role");

        IEnumerable<string>? roles = await _userRepository.GetUserRolesAsync(user);
        string token = _jwtTokenService.GenerateToken(user, roles);
        return Ok(new AddRoleResponse(addRoleRequest.Name, token));
    }

    [HttpPost("RemoveRole")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<RemoveRoleResponse>> RemoveRole(RemoveRoleRequest removeRoleRequest)
    {
        ApplicationUser? user = await _userRepository.GetByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null) return NotFound();

        bool success = await _userRepository.RemoveRole(user, removeRoleRequest.Name);
        if (!success) return BadRequest("Could not remove role");

        IEnumerable<string>? roles = await _userRepository.GetUserRolesAsync(user);
        string token = _jwtTokenService.GenerateToken(user, roles);
        return Ok(new RemoveRoleResponse(removeRoleRequest.Name, token));       
    }

    [HttpGet("User")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<UserResponse>> GetUser()
    {
        ApplicationUser? user = await _userRepository.GetByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if(user == null) return NotFound();

        IEnumerable<string>? roles = await _userRepository.GetUserRolesAsync(user);

        UserResponse userResponse = new(
            user.Name, 
            user.Email,
            roles);

        return Ok(userResponse);
    }

    [HttpGet("AdministratorsCanAccess")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public ActionResult AdministratorsCanAccess()
    {
        return Ok("You are an Administrator!");
    }

    [HttpGet("ModeratorsCanAccess")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Moderator")]
    public ActionResult ModeratorsCanAccess()
    {
        return Ok("You are a Moderator!");
    }

    [HttpGet("UsersCanAccess")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
    public ActionResult UsersCanAccess()
    {
        return Ok("You are an User!");
    }
    public record LoginRequest(string Email, string Password);
    public record LoginResponse(string Token, string RefreshToken);
    public record RegisterRequest(string Name, string Email, string Password, string PasswordConfirmation);
    public record RegisterResponse(string Name, string Email, string Token, string RefreshToken);
    public record RefreshTokenRequest(string RefreshToken);
    public record RefreshTokenResponse(string Token, string RefreshToken);
    public record UserResponse(string Name, string Email, IEnumerable<string> Roles);
    public record AddRoleRequest(string Name);
    public record AddRoleResponse(string Name, string Token);
    public record RemoveRoleRequest(string Name);
    public record RemoveRoleResponse(string Name, string Token);
}
