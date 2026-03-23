using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EstadoController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstadoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> LerEstados()
    {
        return Ok(await _context.Set<Estado>().ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CriarEstado(Estado estado)
    {
        _context.Set<Estado>().Add(estado);
        await _context.SaveChangesAsync();
        return Ok(estado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarEstado(int id)
    {
        var estado = await _context.Set<Estado>().FindAsync(id);
        if (estado == null) return NotFound();

        _context.Set<Estado>().Remove(estado);
        await _context.SaveChangesAsync();
        return Ok();
    }
}