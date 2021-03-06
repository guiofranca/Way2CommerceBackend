using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Mysql.Identity;
using Mysql.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Service;

public class JwtTokenService
{
    private byte[] Secret { get; set; }
    private int TokenExpirationInMinutes { get; set; }
    private int RefreshTokenExpiryInDays { get; set; }
    private readonly RefreshTokenRepository _refreshTokenRepository;

    //https://busk.blog/2022/01/31/authentication-with-jwt-in-net-6/
    //https://dev.to/moe23/refresh-jwt-with-refresh-tokens-in-asp-net-core-5-rest-api-step-by-step-3en5
    //https://www.c-sharpcorner.com/article/jwt-authentication-with-refresh-tokens-in-net-6-0/

    public JwtTokenService(IConfiguration config, RefreshTokenRepository refreshTokenRepository)
    {
        Secret = Encoding.UTF8.GetBytes(config.GetValue<string>("JWT:Secret"));
        TokenExpirationInMinutes = config.GetValue<int>("JWT:TokenExpirationInMinutes");
        RefreshTokenExpiryInDays = config.GetValue<int>("JWT:RefreshTokenExpiryInDays");
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<string> GenerateToken(ApplicationUser user, IEnumerable<string> roles, string? refreshToken = null)
    {
        string expirationTimestamp = new DateTimeOffset(DateTime.Now.AddMinutes(TokenExpirationInMinutes)).ToUnixTimeSeconds().ToString();

        if(string.IsNullOrEmpty(refreshToken)) refreshToken = await GenerateRefreshToken(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, expirationTimestamp),
            new Claim(JwtRegisteredClaimNames.Jti, refreshToken),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }

        var header = new JwtHeader(
            new SigningCredentials(
                new SymmetricSecurityKey(Secret),
                    SecurityAlgorithms.HmacSha256));

        var payload = new JwtPayload(claims);

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshToken(ApplicationUser user)
    {
        RefreshToken refreshToken = new RefreshToken();
        refreshToken.User = user;
        refreshToken.Expiry = DateTime.Now.AddDays(RefreshTokenExpiryInDays);

        bool success = await _refreshTokenRepository.Create(refreshToken);
        if (success) return refreshToken.Id.ToString();

        throw new Exception("Could not generate refresh token");
    }

    public string GetPropertyFromToken(string? token, string claim)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Secret),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        //ClaimTypes.NameIdentifier

        if (!principal.HasClaim(c => c.Type == claim))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal.Claims.First(c => c.Type == claim).Value;
    }

    public async Task LogoutUser(ApplicationUser user)
    {
        await _refreshTokenRepository.RemoveAllRefreshTokensFromUser(user);
    }
}
