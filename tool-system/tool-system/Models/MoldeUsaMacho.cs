using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("molde_usa_macho")]
public class MoldeUsaMacho
{
    [Column("id")]
    public int Id { get; set; }

    [Column("molde_id")]
    public int MoldeId { get; set; }

    [Column("macho_id")]
    public int MachoId { get; set; }
}