using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API;

public class TokenService(IConfiguration confg) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = confg["Tokenkey"] ?? throw new Exception("cannot acess tokenkey from appsetting.json");
        if(tokenKey.Length <64) throw new Exception("your tokenKey needs to be longer");
        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>
        {
            // new(ClaimTypes.NameIdentifier , user.UserName )
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        var creds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
