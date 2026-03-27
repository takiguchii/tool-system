using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolingSystem.API.Models;
using ToolingSystem.API.Services;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MachoController : ControllerBase
{
    private readonly MachoService _service;

    public MachoController(MachoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> LerMachos() => Ok(await _service.ObterTodosAsync());

    [HttpPost]
    public async Task<IActionResult> CriarMacho([FromQuery] int moldeId, [FromBody] Macho macho)
    {
        var novoMacho = await _service.CriarVinculadoAsync(moldeId, macho);
        return Ok(novoMacho);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarMacho(int id, Macho machoAtualizado)
    {
        var resultado = await _service.AtualizarAsync(id, machoAtualizado);
        if (!resultado.Sucesso) return BadRequest(resultado.Mensagem);
        return Ok(resultado.Dados);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMacho(int id)
    {
        var resultado = await _service.DeletarAsync(id);
        if (!resultado.Sucesso) return NotFound(resultado.Mensagem);
        return Ok("Macho deletado com sucesso.");
    }
}