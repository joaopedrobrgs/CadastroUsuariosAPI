using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsuarioApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AcessoController : ControllerBase
{
    [HttpGet]
    [Route("")]
    //Passando as politicas de autorização que devem estar contidas nessa rota:
    [Authorize(Policy = "IdadeMinima")]
    public IActionResult ValidarAcesso()
    {
        return Ok("Acesso Permitido");
    }


}
