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
    private readonly IWebHostEnvironment _ambiente;

    public MoldeController(AppDbContext context, IWebHostEnvironment ambiente)
    {
        _context = context;
        _ambiente = ambiente;
    }

    [HttpGet]
    public async Task<IActionResult> LerMoldes()
    {
        return Ok(await _context.Moldes.ToListAsync());
    }
    
    [HttpGet("{id}/machos")]
    public async Task<IActionResult> ObterMachosDoMolde(int id)
    {
        var machos = await _context.Set<MoldeUsaMacho>()
            .Where(v => v.MoldeId == id)
            .Join(_context.Machos,
                  v => v.MachoId,
                  m => m.Id,
                  (v, m) => m)
            .ToListAsync();

        return Ok(machos);
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

        var moldeAtual = await _context.Moldes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (moldeAtual == null) return NotFound("O molde que você está tentando editar não foi encontrado.");

        _context.Entry(moldeAtualizado).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        DeletarImagemSeNecessario(moldeAtual.Imagem1, moldeAtualizado.Imagem1);
        DeletarImagemSeNecessario(moldeAtual.Imagem2, moldeAtualizado.Imagem2);
        DeletarImagemSeNecessario(moldeAtual.Imagem3, moldeAtualizado.Imagem3);

        return Ok(moldeAtualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMolde(int id)
    {
        var molde = await _context.Moldes.FindAsync(id);
        if (molde == null) return NotFound("Molde não encontrado.");

        _context.Moldes.Remove(molde);
        await _context.SaveChangesAsync();
        
        DeletarImagemSeNecessario(molde.Imagem1, null);
        DeletarImagemSeNecessario(molde.Imagem2, null);
        DeletarImagemSeNecessario(molde.Imagem3, null);

        return Ok("Molde deletado com sucesso.");
    }
    
    private void DeletarImagemSeNecessario(string? nomeImagemAntiga, string? nomeImagemNova)
    {
        if (string.IsNullOrEmpty(nomeImagemAntiga)) return;
        if (nomeImagemAntiga == nomeImagemNova) return;

        try
        {
            var pastaRaiz = _ambiente.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var caminhoCompleto = Path.Combine(pastaRaiz, "imagens", "moldes", nomeImagemAntiga);
            
            if (System.IO.File.Exists(caminhoCompleto))
            {
                System.IO.File.Delete(caminhoCompleto);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível apagar a imagem antiga '{nomeImagemAntiga}'. Erro: {ex.Message}");
        }
    }
}