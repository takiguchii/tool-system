using Microsoft.EntityFrameworkCore;
using ToolingSystem.API.Models;

namespace tool_system.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Macho> Machos { get; set; }
    public DbSet<Molde> Moldes { get; set; }
    public DbSet<MoldeUsaMacho> MoldeUsaMachos { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}