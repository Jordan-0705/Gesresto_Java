using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public ImageController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier envoyé.");

        // Appel du service Cloudinary
        string? url = await _cloudinaryService.UploadImageAsync(file);

        if (string.IsNullOrEmpty(url))
            return StatusCode(500, "Erreur lors de l'upload de l'image.");

        // Retourne l'URL de l'image uploadée
        return Ok(new { ImageUrl = url });
    }
}
