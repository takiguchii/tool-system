using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;

namespace ToolingSystem.API.Services;

public class MachoService
{
    private readonly AppDbContext _context;

    public MachoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Macho>> ObterTodosAsync() => await _context.Machos.ToListAsync();

    public async Task<Macho> CriarVinculadoAsync(int moldeId, Macho macho)
    {
        _context.Machos.Add(macho);
        await _context.SaveChangesAsync();

        var vinculo = new MoldeUsaMacho { MoldeId = moldeId, MachoId = macho.Id };
        _context.Add(vinculo);
        await _context.SaveChangesAsync();

        return macho;
    }

    public async Task<(bool Sucesso, string Mensagem, Macho? Dados)> AtualizarAsync(int id, Macho machoAtualizado)
    {
        if (id != machoAtualizado.Id) return (false, "Os IDs não combinam.", null);

        _context.Entry(machoAtualizado).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return (true, "", machoAtualizado);
    }

    public async Task<(bool Sucesso, string Mensagem)> DeletarAsync(int id)
    {
        var macho = await _context.Machos.FindAsync(id);
        if (macho == null) return (false, "Macho não encontrado.");

        var vinculos = await _context.Set<MoldeUsaMacho>().Where(v => v.MachoId == id).ToListAsync();
        if (vinculos.Any()) _context.Set<MoldeUsaMacho>().RemoveRange(vinculos);

        _context.Machos.Remove(macho);
        await _context.SaveChangesAsync();

        return (true, "");
    }
}