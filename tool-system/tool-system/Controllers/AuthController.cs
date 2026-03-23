using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using ToolingSystem.API.Models;
using ToolingSystem.API.Services;

namespace ToolingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(UsuarioLoginDto requisicao)
    {
        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Username == requisicao.Username);
        if (usuarioExiste) return BadRequest("Este usuário já existe.");

        var usuario = new Usuario
        {
            Username = requisicao.Username,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(requisicao.Senha),
            Regra = requisicao.Regra 
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok("Usuário criado com sucesso!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UsuarioLoginDto requisicao)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == requisicao.Username);
        if (usuario == null) return BadRequest("Usuário não encontrado.");

        // Blindagem: Tenta verificar a criptografia. Se for uma senha injetada manual (pura), faz a verificação simples.
        bool senhaCorreta = false;
        try 
        {
            senhaCorreta = BCrypt.Net.BCrypt.Verify(requisicao.Senha, usuario.SenhaHash);
        } 
        catch 
        {
            senhaCorreta = (requisicao.Senha == usuario.SenhaHash);
        }

        if (!senhaCorreta) return BadRequest("Senha incorreta.");

        var chaveSecreta = _configuration.GetValue<string>("Jwt:Chave");
        var token = TokenService.GerarToken(usuario, chaveSecreta!);

        return Ok(new 
        { 
            token = token,
            perfil = usuario.Perfil ?? "Admin" 
        });
    }

    public class UsuarioLoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Regra { get; set; } = "Encarregado";
    }
}