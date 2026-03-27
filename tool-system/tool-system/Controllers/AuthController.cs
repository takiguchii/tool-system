using Microsoft.AspNetCore.Mvc;
using ToolingSystem.API.Services;

namespace ToolingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(UsuarioLoginDto requisicao)
    {
        var resultado = await _service.RegistrarAsync(requisicao);
        if (!resultado.Sucesso) return BadRequest(resultado.Mensagem);
        
        return Ok(resultado.Mensagem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UsuarioLoginDto requisicao)
    {
        var resultado = await _service.LoginAsync(requisicao);
        if (!resultado.Sucesso) return BadRequest(resultado.Mensagem);

        return Ok(resultado.Dados);
    }

    public class UsuarioLoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Regra { get; set; } = "Encarregado";
    }
}