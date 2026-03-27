using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolingSystem.API.Models;
using ToolingSystem.API.Services;
using ToolingSystem.API.Dtos;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MoldeController : ControllerBase
{
    private readonly MoldeService _service;

    public MoldeController(MoldeService service)
    {
        _service = service;
    }

    [HttpGet("{id}/machos")]
    public async Task<IActionResult> ObterMachosDoMolde(int id) => Ok(await _service.ObterMachosDoMoldeAsync(id));

    [HttpGet]
    public async Task<IActionResult> LerMoldes([FromQuery] FiltroMoldeDto filtro)
    {
        if (filtro.Pagina < 1) filtro.Pagina = 1;
        if (filtro.TamanhoPagina > 50) filtro.TamanhoPagina = 10;

        var resultado = await _service.ObterTodosPaginadoAsync(filtro);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> CriarMolde(Molde molde)
    {
        var resultado = await _service.CriarAsync(molde);
        if (!resultado.Sucesso) return BadRequest(resultado.Mensagem);
        return Ok(resultado.Dados);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarMolde(int id, Molde moldeAtualizado)
    {
        var resultado = await _service.AtualizarAsync(id, moldeAtualizado);
        if (!resultado.Sucesso) return BadRequest(resultado.Mensagem);
        return Ok(resultado.Dados);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMolde(int id)
    {
        try
        {
            var resultado = await _service.DeletarAsync(id);
            if (!resultado.Sucesso) return NotFound(resultado.Mensagem);
            return Ok("Molde deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno da API: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
}