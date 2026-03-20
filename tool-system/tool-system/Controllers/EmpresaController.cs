using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

[Authorize] 
[ApiController]
[Route("api/[controller]")]
public class EmpresaController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmpresaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> PegarTodasAsEmpresas()
    {
        var empresas = await _context.Empresas.ToListAsync();
        return Ok(empresas);
    }

    [HttpPost]
    public async Task<IActionResult> CriarEmpresa(Empresa empresa)
    {
        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();
        return Ok(empresa);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarEmpresa(int id, Empresa empresaAtualizada)
    {
        if (id != empresaAtualizada.Id) return BadRequest("Os IDs não combinam.");

        _context.Entry(empresaAtualizada).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(empresaAtualizada);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarEmpresa(int id)
    {
        var empresa = await _context.Empresas.FindAsync(id);
        if (empresa == null) return NotFound("Empresa não encontrada.");

        _context.Empresas.Remove(empresa);
        await _context.SaveChangesAsync();

        return Ok("Empresa deletada com sucesso.");
    }
}