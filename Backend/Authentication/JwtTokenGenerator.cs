using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Authentication;

public sealed class JwtTokenGenerator(IConfiguration configuration)
{
    public string GetJwtToken(string username, string role)
    {
        var issuer = configuration["AuthenticationOptions:Issuer"];
        var audience = configuration["AuthenticationOptions:Audience"];
        var secretKey = configuration["AuthenticationOptions:SigningKey"];
        var expirationHours = double.Parse(configuration["AuthenticationOptions:ExpirationHours"]!);

        // Symmetric security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        // Signing credentials
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims (add your custom claims here)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username), // User identifier
            new Claim(ClaimTypes.Role, role),  // Role
        };

        // Token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expirationHours), // Token expiration time
            Issuer = issuer, // Specify Issuer
            Audience = audience, // Specify Audience
            SigningCredentials = credentials
        };

        // Token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the token string
        return tokenHandler.WriteToken(token);
    }
}