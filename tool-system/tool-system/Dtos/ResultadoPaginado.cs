namespace ToolingSystem.API.Dtos;

public class ResultadoPaginado<T>
{
    public List<T> Dados { get; set; } = new List<T>();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
}