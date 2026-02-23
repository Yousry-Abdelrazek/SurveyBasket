
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Authentication;

public class JwtProvider : IJwtProvider
{
    //public (string token, int expireIn) GenerateToken(ApplicationUser user)
    //{
    //    Claim[] claims = new Claim[] 
    //    {
    //        new (JwtRegisteredClaimNames.Sub, user.Id),
    //        new (JwtRegisteredClaimNames.Email, user.Email!),
    //        new (JwtRegisteredClaimNames.GivenName , user.FirstName),
    //        new (JwtRegisteredClaimNames.FamilyName , user.LastName),
    //        new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //    };


    //    // Genereate key to encode the token and decodo it too => SHA256

    //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YvQfJrq4sPA30c5LsaENXwfuF6qDPGej"));


    //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


    //    var expireIn = 30;

    //    var expirationDate = DateTime.UtcNow.AddMinutes(expireIn);

    //    var token = new JwtSecurityToken(
    //        issuer: "SurveyBasketApp",
    //        audience: "SurveyBasketApp users",
    //        claims: claims,
    //        expires: expirationDate,
    //        signingCredentials: signingCredentials


    //        );

    //    return (token : new JwtSecurityTokenHandler().WriteToken(token), expireIn: expireIn);


    //}
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
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YvQfJrq4sPA30c5LsaENXwfuF6qDPGej"));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expireIn = 30;
        var expirationDate = DateTime.UtcNow.AddMinutes(expireIn);

        var token = new JwtSecurityToken(
            issuer: "SurveyBasketApp",
            audience : "SurveyBasketApp users",
            signingCredentials: signingCredentials,
            expires: expirationDate,
            claims: claims
        );

        return (token : new JwtSecurityTokenHandler().WriteToken(token) , expireIn: expireIn);


    }
}
