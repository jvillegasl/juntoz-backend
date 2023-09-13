using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserStoreApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserStoreApi.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly string TokenSecret;
    private static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(1);
    private readonly string Issuer;
    private readonly string Audience;
    private readonly UsersService _usersService;

    public AuthController(IConfiguration config, UsersService usersService)
    {
        TokenSecret = config["JwtSettings:Key"];
        Issuer = config["JwtSettings:Issuer"];
        Audience = config["JwtSettings:Audience"];
        _usersService = usersService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUser request)
    {
        bool result = await _usersService.CreateAsync(request);

        if (!result)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { message = "User already exists" });
        }

        return Ok();

    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUser request)
    {
        User? user = await AuthenticateUser(request);

        if (user == null)
        {
            return StatusCode(401);
        }

        string token = GenerateToken(user);

        return Ok(new
        {
            token,
            expiresIn = TokenLifeTime.TotalMinutes
        });
    }

    private string GenerateToken(User user)
    {
        List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.Add(TokenLifeTime),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return jwt;
    }

    private async Task<User?> AuthenticateUser(LoginUser request)
    {
        var currentUser = await _usersService.FindByUsernameAsync(request.Username);

        if (currentUser == null) { return null; }

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, currentUser.Password);

        if (verified == false)
        {
            return null;
        }

        return currentUser;
    }
}