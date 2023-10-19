using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsuarioApi.Models;

namespace UsuarioApi.Services;

public class TokenService
{

    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Usuario usuario)
    {
        //Informações que estarão armazenadas no TOKEN:
        Claim[] claims = new Claim[]
        {
            new Claim("id", usuario.Id),
            new Claim("username", usuario.UserName),
            new Claim(ClaimTypes.DateOfBirth, usuario.DataNascimento.ToString()),
            //Retornando momento em que foi feito o login:
            new Claim("loginTimestamp", DateTime.UtcNow.ToString())
        };

        //Tempo de expiração do Token:
        DateTime expiresTime = DateTime.Now.AddMinutes(10);

        //Credenciais de Login:
        ////Gerando chave (tomar cuidado com tamanho da chave, que deve ter uma quantidade mínima de caracteres):
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["SymmetricSecurityKey"]));
        ////Utilizando chave para gerar um credencial:
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        //Gerando token:
        JwtSecurityToken token = new(
                expires: expiresTime,
                claims: claims,
                signingCredentials: signingCredentials
            );

        //Convertendo o objeto token para o formato de string/cadeia de caracteres):
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}