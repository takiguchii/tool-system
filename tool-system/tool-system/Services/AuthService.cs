using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;
using ToolingSystem.API.Services; 
using static ToolingSystem.API.Controllers.AuthController; 
namespace ToolingSystem.API.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioLoginDto requisicao)
    {
        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Username == requisicao.Username);
        if (usuarioExiste) return (false, "Este usuário já existe.");

        var usuario = new Usuario
        {
            Username = requisicao.Username,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(requisicao.Senha),
            Regra = requisicao.Regra,
            Perfil = "Consultor" 
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return (true, "Usuário criado com sucesso!");
    }

    public async Task<(bool Sucesso, string Mensagem, object? Dados)> LoginAsync(UsuarioLoginDto requisicao)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == requisicao.Username);
        if (usuario == null) return (false, "Usuário não encontrado.", null);

        bool senhaCorreta = false;
        try 
        {
            senhaCorreta = BCrypt.Net.BCrypt.Verify(requisicao.Senha, usuario.SenhaHash);
        } 
        catch 
        {
            senhaCorreta = (requisicao.Senha == usuario.SenhaHash);
        }

        if (!senhaCorreta) return (false, "Senha incorreta.", null);

        var chaveSecreta = _configuration.GetValue<string>("Jwt:Chave");
        var token = TokenService.GerarToken(usuario, chaveSecreta!);

        var resposta = new 
        { 
            token = token,
            perfil = usuario.Perfil ?? "Admin" 
        };

        return (true, "", resposta);
    }
}