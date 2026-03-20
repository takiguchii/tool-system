using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MoldeController : ControllerBase
{
    private readonly AppDbContext _context;

    public MoldeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> LerMoldes()
    {
        return Ok(await _context.Moldes.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CriarMolde(Molde molde)
    {
        if (molde.EmpresaId.HasValue && !await _context.Empresas.AnyAsync(e => e.Id == molde.EmpresaId))
            return BadRequest("A empresa informada não existe.");

        if (molde.CategoriaId.HasValue && !await _context.Categorias.AnyAsync(c => c.Id == molde.CategoriaId))
            return BadRequest("A categoria informada não existe.");

        _context.Moldes.Add(molde);
        await _context.SaveChangesAsync();
        return Ok(molde);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarMolde(int id, Molde moldeAtualizado)
    {
        if (id != moldeAtualizado.Id) return BadRequest("Os IDs não combinam.");

        if (moldeAtualizado.EmpresaId.HasValue && !await _context.Empresas.AnyAsync(e => e.Id == moldeAtualizado.EmpresaId))
            return BadRequest("A empresa informada não existe.");

        if (moldeAtualizado.CategoriaId.HasValue && !await _context.Categorias.AnyAsync(c => c.Id == moldeAtualizado.CategoriaId))
            return BadRequest("A categoria informada não existe.");

        _context.Entry(moldeAtualizado).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(moldeAtualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMolde(int id)
    {
        var molde = await _context.Moldes.FindAsync(id);
        if (molde == null) return NotFound("Molde não encontrado.");

        _context.Moldes.Remove(molde);
        await _context.SaveChangesAsync();

        return Ok("Molde deletado com sucesso.");
    }
}