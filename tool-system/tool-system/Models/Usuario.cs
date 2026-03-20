using System.ComponentModel.DataAnnotations.Schema;

namespace ToolingSystem.API.Models;

[Table("usuario")]
public class Usuario
{
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("senha_hash")]
    public string SenhaHash { get; set; } = string.Empty;

    [Column("regra")]
    public string Regra { get; set; } = string.Empty; 
}