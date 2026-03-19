using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Controllers;

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
        var machos = await _context.Machos.ToListAsync();
        return Ok(machos);
    }

    [HttpPost]
    public async Task<IActionResult> CriarMacho(Macho macho)
    {
        _context.Machos.Add(macho);
        await _context.SaveChangesAsync();
        return Ok(macho);
    }
}