using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Services;

public static class TokenService
{
    public static string GerarToken(Usuario usuario, string chaveSecreta)
    {
        var manipuladorToken = new JwtSecurityTokenHandler();
        var chave = Encoding.ASCII.GetBytes(chaveSecreta);
        
        var descricaoToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Regra)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = manipuladorToken.CreateToken(descricaoToken);
        return manipuladorToken.WriteToken(token);
    }
}