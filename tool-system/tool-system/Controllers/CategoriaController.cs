using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> LerCategorias()
    {
        return Ok(await _context.Categorias.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CriarCategoria(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return Ok(categoria);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarCategoria(int id, Categoria categoriaAtualizada)
    {
        if (id != categoriaAtualizada.Id) return BadRequest("Os IDs não combinam.");

        _context.Entry(categoriaAtualizada).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(categoriaAtualizada);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound("Categoria não encontrada.");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok("Categoria deletada com sucesso.");
    }
}