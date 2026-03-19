using ToolingSystem.API.Models;

namespace tool_system.Data;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Empresa> Empresas { get; set; }
}