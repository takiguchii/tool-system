using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("empresa")]
public class Empresa
{
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("cidade")]
    public string? Cidade { get; set; }
}