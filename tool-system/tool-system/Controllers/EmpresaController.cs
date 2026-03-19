
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using tool_system.Models;

namespace ToolingSystem.API.Controllers;

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
}