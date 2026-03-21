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
    public async Task<IActionResult> SalvarImagem(string tipo, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Nenhum arquivo enviado.");

        if (tipo != "moldes" && tipo != "machos")
            return BadRequest("O tipo deve ser 'moldes' ou 'machos'.");

        var extensao = Path.GetExtension(arquivo.FileName);
        var nomeUnico = Guid.NewGuid().ToString() + extensao;

        var caminhoPasta = Path.Combine(_ambiente.WebRootPath, "imagens", tipo);
        var caminhoCompleto = Path.Combine(caminhoPasta, nomeUnico);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        return Ok(new { nomeImagem = nomeUnico });
    }
}