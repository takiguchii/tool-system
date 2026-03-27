namespace ToolingSystem.API.Dtos;

public class FiltroMoldeDto
{
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public string? TermoBusca { get; set; }
}