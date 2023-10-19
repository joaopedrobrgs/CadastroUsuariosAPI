using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UsuarioApi.Data.Dtos;
using UsuarioApi.Models;

namespace UsuarioApi.Services;

public class UsuarioService
{

    private IMapper _mapper;
    private UserManager<Usuario> _userManager;
    private SignInManager<Usuario> _signInManager;
    private TokenService _tokenService;

    public UsuarioService(UserManager<Usuario> userManager, IMapper mapper, SignInManager<Usuario> signInManager, TokenService tokenService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    //Método para cadastrar um usuário no banco de dados:
    public async Task CadastrarUsuario(CreateUsuarioDto usuarioDto)
    {
        //Convertendo usuário de CreateUsuarioDto para Usuario:
        Usuario usuario = _mapper.Map<Usuario>(usuarioDto);
        //Aguardando resultado da requisição para Cadastrar objeto no banco de dados:
        IdentityResult resultado = await _userManager.CreateAsync(usuario, usuarioDto.Password);
        if (!resultado.Succeeded)
        {
            //Retornando se houve algum erro nesse cadastro:
            throw new ApplicationException("Falha ao cadastrar usuário");
        }
        _tokenService.GenerateToken(usuario);

    }

    //Método para logar usuário no banco de dados:
    public async Task<string> LogarUsuario(LoginUsuarioDto usuarioDto)
    {
        //Aguardando resultado da requisição para Logar:
        SignInResult resultado = await _signInManager.PasswordSignInAsync(usuarioDto.Username, usuarioDto.Password, false, false);
        //Retornando se houve algum erro nesse login:
        if (!resultado.Succeeded)
        {
            throw new ApplicationException("Usuário não autenticado");
        }

        //Pegando dados do usuário que queremos passar para o TOKEN:
        var usuario = _signInManager
            .UserManager
            .Users
            .FirstOrDefault(usuario => usuarioDto.Username.ToUpper() == usuario.NormalizedUserName); //Colocamos ambos os criterios de comparação em letra maiuscula para evitar problemas

        //Verificando se foi encontrado usuário de fato (apenas para que compilador não reclame):
        if(usuario == null)
        {
            throw new ApplicationException("Usuário não encontrado");
        }

        //Executando método de gerar token e atribuindo resultado (que vai vir em forma de string) a uma variável:
        var token = _tokenService.GenerateToken(usuario);

        //Retornando token:
        return token;

    }
}
