using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsuarioApi.Data;
using UsuarioApi.Data.Dtos;
using UsuarioApi.Models;
using UsuarioApi.Services;

namespace UsuarioApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{

    private UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost, Route("cadastro")]
    public async Task<IActionResult> CadastroUsuário([FromBody] CreateUsuarioDto usuarioDto)
    {
        await _usuarioService.CadastrarUsuario(usuarioDto);
        return Ok("Usuário Cadastrado");
    }

    [HttpPost, Route("login")]
    public async Task<IActionResult> LoginUsuario([FromBody] LoginUsuarioDto usuarioDto)
    {
        //Executando serviço de logar usuário que vai nos retornar token se der tudo certo:
        var token = await _usuarioService.LogarUsuario(usuarioDto);
        //Retornando este token ao front-end:
        return Ok(token);
    }
}
