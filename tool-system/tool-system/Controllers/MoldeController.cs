using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

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
        var moldes = await _context.Moldes.ToListAsync();
        return Ok(moldes);
    }

    [HttpPost]
    public async Task<IActionResult> CriarMolde(Molde molde)
    {
        if (molde.EmpresaId.HasValue)
        {
            var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == molde.EmpresaId);
            if (!empresaExiste) return BadRequest("A empresa informada não existe.");
        }

        if (molde.CategoriaId.HasValue)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == molde.CategoriaId);
            if (!categoriaExiste) return BadRequest("A categoria informada não existe.");
        }

        _context.Moldes.Add(molde);
        await _context.SaveChangesAsync();
        return Ok(molde);
    }
}