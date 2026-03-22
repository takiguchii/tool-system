using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ToolingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _ambiente;

    public UploadController(IWebHostEnvironment ambiente)
    {
        _ambiente = ambiente;
    }

    [HttpPost("imagem/{tipo}")]
    public async Task<IActionResult> SalvarImagem([FromRoute] string tipo, [FromForm] IFormFile arquivo)
    {
        try 
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("O Vue.js enviou um pacote vazio para a API.");

            if (tipo != "moldes" && tipo != "machos")
                return BadRequest("O tipo deve ser 'moldes' ou 'machos'.");

            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeUnico = Guid.NewGuid().ToString() + extensao;

            var pastaRaiz = _ambiente.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var caminhoPasta = Path.Combine(pastaRaiz, "imagens", tipo);

            if (!Directory.Exists(caminhoPasta))
            {
                Directory.CreateDirectory(caminhoPasta);
            }

            var caminhoCompleto = Path.Combine(caminhoPasta, nomeUnico);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return Ok(new { nomeImagem = nomeUnico });
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(500, "Erro Crítico: O Docker (Linux) bloqueou a permissão de escrita na pasta de imagens.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro na API: {ex.Message}");
        }
    }
}