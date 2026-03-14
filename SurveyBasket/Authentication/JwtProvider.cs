
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    

    public (string token, int expireIn) GenerateToken(ApplicationUser user)
    {

        // Add Claims to the token  
        Claim[] claims = new Claim[]
        {
            new (JwtRegisteredClaimNames.Sub , user.Id),
            new (JwtRegisteredClaimNames.Email , user.Email!),
            new (JwtRegisteredClaimNames.GivenName , user.FirstName),
            new (JwtRegisteredClaimNames.FamilyName , user.LastName),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        // Genereate key to encode the token and decodo it too => SHA256
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expireIn = _options.ExpireyMinutes;
        var expirationDate = DateTime.UtcNow.AddMinutes(expireIn);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience : _options.Audience,
            signingCredentials: signingCredentials,
            expires: expirationDate,
            claims: claims
        );

        return (token : new JwtSecurityTokenHandler().WriteToken(token) , expireIn: expireIn);


    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,

                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

        }
        catch
        {
            return null; 
        }

    }
}
