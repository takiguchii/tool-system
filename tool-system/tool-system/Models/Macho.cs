using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("macho")]
public class Macho
{
    [Column("id")]
    public int Id { get; set; }

    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;
    public string? Imagem1 { get; set; }
    public string? Imagem2 { get; set; }
    public string? Imagem3 { get; set; }
}