using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("molde")]
public class Molde
{
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("prateleira")]
    public string? Prateleira { get; set; }

    [Column("empresa_id")]
    public int? EmpresaId { get; set; }

    [Column("categoria_id")]
    public int? CategoriaId { get; set; }
}