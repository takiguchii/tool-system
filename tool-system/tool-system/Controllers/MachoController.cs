using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MachoController : ControllerBase
{
    private readonly AppDbContext _context;

    public MachoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> LerMachos()
    {
        return Ok(await _context.Machos.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CriarMacho(Macho macho)
    {
        _context.Machos.Add(macho);
        await _context.SaveChangesAsync();
        return Ok(macho);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarMacho(int id, Macho machoAtualizado)
    {
        if (id != machoAtualizado.Id) return BadRequest("Os IDs não combinam.");

        _context.Entry(machoAtualizado).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(machoAtualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMacho(int id)
    {
        var macho = await _context.Machos.FindAsync(id);
        if (macho == null) return NotFound("Macho não encontrado.");

        _context.Machos.Remove(macho);
        await _context.SaveChangesAsync();

        return Ok("Macho deletado com sucesso.");
    }
}