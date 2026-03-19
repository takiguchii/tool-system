using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("estado")]
public class Estado
{
    [Column("id")]
    public int Id { get; set; }

    [Column("estado_atual")]
    public string EstadoAtual { get; set; } = string.Empty;

    [Column("data_entrada")]
    public DateTime? DataEntrada { get; set; }

    [Column("data_saida")]
    public DateTime? DataSaida { get; set; }

    [Column("molde_id")]
    public int MoldeId { get; set; }
}