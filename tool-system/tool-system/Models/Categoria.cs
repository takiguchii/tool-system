using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("categoria")]
public class Categoria
{
    [Column("id")]
    public int Id { get; set; }

    [Column("setor")]
    public string Setor { get; set; } = string.Empty;
}