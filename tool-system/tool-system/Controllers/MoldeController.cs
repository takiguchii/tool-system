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
        _context.Moldes.Add(molde);
        await _context.SaveChangesAsync();
        return Ok(molde);
    }
}