using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Services;

public class MoldeService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _ambiente;

    public MoldeService(AppDbContext context, IWebHostEnvironment ambiente)
    {
        _context = context;
        _ambiente = ambiente;
    }

    public async Task<List<Molde>> ObterTodosAsync() => await _context.Moldes.ToListAsync();

    public async Task<List<Macho>> ObterMachosDoMoldeAsync(int id)
    {
        return await _context.Set<MoldeUsaMacho>()
            .Where(v => v.MoldeId == id)
            .Join(_context.Machos, v => v.MachoId, m => m.Id, (v, m) => m)
            .ToListAsync();
    }

    public async Task<(bool Sucesso, string Mensagem, Molde? Dados)> CriarAsync(Molde molde)
    {
        if (molde.EmpresaId.HasValue && !await _context.Empresas.AnyAsync(e => e.Id == molde.EmpresaId))
            return (false, "A empresa informada não existe.", null);

        if (molde.CategoriaId.HasValue && !await _context.Categorias.AnyAsync(c => c.Id == molde.CategoriaId))
            return (false, "A categoria informada não existe.", null);

        if (molde.Status == "Ativo") molde.DataEntrada = DateTime.Now;
        else if (molde.Status == "Desativado") molde.DataSaida = DateTime.Now;

        _context.Moldes.Add(molde);
        await _context.SaveChangesAsync();
        return (true, "", molde);
    }

    public async Task<(bool Sucesso, string Mensagem, Molde? Dados)> AtualizarAsync(int id, Molde moldeAtualizado)
    {
        if (id != moldeAtualizado.Id) return (false, "Os IDs não combinam.", null);

        if (moldeAtualizado.EmpresaId.HasValue && !await _context.Empresas.AnyAsync(e => e.Id == moldeAtualizado.EmpresaId))
            return (false, "A empresa informada não existe.", null);

        if (moldeAtualizado.CategoriaId.HasValue && !await _context.Categorias.AnyAsync(c => c.Id == moldeAtualizado.CategoriaId))
            return (false, "A categoria informada não existe.", null);

        var moldeAtual = await _context.Moldes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (moldeAtual == null) return (false, "Molde não encontrado.", null);

        if (moldeAtualizado.Status == "Ativo" && moldeAtual.Status != "Ativo") 
        {
            moldeAtualizado.DataEntrada = DateTime.Now;
            moldeAtualizado.DataSaida = null; 
        } 
        else if (moldeAtualizado.Status == "Desativado" && moldeAtual.Status != "Desativado") 
        {
            moldeAtualizado.DataSaida = DateTime.Now;
            moldeAtualizado.DataEntrada = moldeAtual.DataEntrada; 
        }
        else 
        {
            moldeAtualizado.DataEntrada = moldeAtual.DataEntrada;
            moldeAtualizado.DataSaida = moldeAtual.DataSaida;
        }

        _context.Entry(moldeAtualizado).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        DeletarImagemSeNecessario(moldeAtual.Imagem1, moldeAtualizado.Imagem1);
        DeletarImagemSeNecessario(moldeAtual.Imagem2, moldeAtualizado.Imagem2);
        DeletarImagemSeNecessario(moldeAtual.Imagem3, moldeAtualizado.Imagem3);

        return (true, "", moldeAtualizado);
    }

    public async Task<(bool Sucesso, string Mensagem)> DeletarAsync(int id)
    {
        var molde = await _context.Moldes.FindAsync(id);
        if (molde == null) return (false, "Molde não encontrado.");

        var vinculos = await _context.Set<MoldeUsaMacho>().Where(v => v.MoldeId == id).ToListAsync();
        if (vinculos.Any()) _context.Set<MoldeUsaMacho>().RemoveRange(vinculos);

        var historicoEstados = await _context.Set<Estado>().Where(e => e.MoldeId == id).ToListAsync();
        if (historicoEstados.Any()) _context.Set<Estado>().RemoveRange(historicoEstados);

        await _context.SaveChangesAsync();

        DeletarImagemSeNecessario(molde.Imagem1, null);
        DeletarImagemSeNecessario(molde.Imagem2, null);
        DeletarImagemSeNecessario(molde.Imagem3, null);

        _context.Moldes.Remove(molde);
        await _context.SaveChangesAsync();

        return (true, "");
    }

    private void DeletarImagemSeNecessario(string? imagemAntiga, string? imagemNova)
    {
        try
        {
            if (!string.IsNullOrEmpty(imagemAntiga) && imagemAntiga != imagemNova)
            {
                var pastaRaiz = _ambiente.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var caminhoArquivo = Path.Combine(pastaRaiz, "imagens", "moldes", imagemAntiga);
                if (System.IO.File.Exists(caminhoArquivo)) System.IO.File.Delete(caminhoArquivo);
            }
        }
        catch { }
    }
}