using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UsuarioApi.Models;

public class Usuario : IdentityUser
{

    public DateTime DataNascimento { get; set; }

    //Com esse construtor vamos ter todas as propriedades da classe IdentityUser + a propriedade DataNascimento aqui da classe Usuario
    public Usuario() : base()
    {
    }
}
