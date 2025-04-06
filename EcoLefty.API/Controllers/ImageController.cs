using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Common.Images.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        if (request.ImageFile == null || request.ImageFile.Length == 0)
            return BadRequest("No file provided");

        var imagePath = await _imageService.UploadImageAsync(request.ImageFile);
        return Ok(new { imagePath });
    }
}