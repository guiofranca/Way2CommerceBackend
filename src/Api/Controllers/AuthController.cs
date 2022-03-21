using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mysql.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository<ApplicationUser> _userRepository;

    public AuthController([FromServices] IUserRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        ApplicationUser? user = await _userRepository.TryLoginAsync(loginRequest.Email, loginRequest.Password);
        if (user != null)
        {
            string token = await GenerateToken(user);
            return Ok(new LoginResponse(token));
        }

        return BadRequest("Invalid username and/or password.");
    }

    [HttpPost("Register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest registerRequest)
    {
        ApplicationUser? user = await _userRepository.RegisterAsync(registerRequest.Name, registerRequest.Email, registerRequest.Password);

        if(user != null)
        {
            string token = await GenerateToken(user);
            return Created("", new RegisterResponse(registerRequest.Name, registerRequest.Email, token));
        }

        return BadRequest("Could not create user");
    }

    [HttpPost("AddRole")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<AddRoleResponse>> AddRole(AddRoleRequest addRoleRequest)
    {
        ApplicationUser? user = await _userRepository.GetByIdAsync(User.Claims.ToList().Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
        if (user == null) return NotFound();
        bool success = await _userRepository.AddRole(user, addRoleRequest.Name);
        if (success) return Ok(new AddRoleResponse(addRoleRequest.Name, await GenerateToken(user)));
        return BadRequest("Could not add role");
    }

    [HttpPost("RemoveRole")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<RemoveRoleResponse>> RemoveRole(RemoveRoleRequest removeRoleRequest)
    {
        ApplicationUser? user = await _userRepository.GetByIdAsync(User.Claims.ToList().Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
        if (user == null) return NotFound();
        bool success = await _userRepository.RemoveRole(user, removeRoleRequest.Name);
        if (success) return Ok(new RemoveRoleResponse(removeRoleRequest.Name, await GenerateToken(user)));
        return BadRequest("Could not remove role");
    }

    [HttpGet("User")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<UserResponse>> GetUser()
    {
        UserResponse user = new(User.Identity.Name, User.Claims.ToList().Where(c => c.Type == ClaimTypes.Email).First().Value);
        return Ok(user);
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

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        IEnumerable<string> roles = await _userRepository.GetUserRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
        };

        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var header = new JwtHeader(
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("ThisKeyMustBeAtLeast16Characters")),
                    SecurityAlgorithms.HmacSha256));

        var payload = new JwtPayload(claims);

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public record LoginRequest(string Email, string Password);
    public record LoginResponse(string Token);
    public record RegisterRequest(string Name, string Email, string Password);
    public record RegisterResponse(string Name, string Email, string Token);
    public record UserResponse(string Name, string Email);
    public record AddRoleRequest(string Name);
    public record AddRoleResponse(string Name, string Token);
    public record RemoveRoleRequest(string Name);
    public record RemoveRoleResponse(string Name, string Token);
}
