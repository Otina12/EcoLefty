using Microsoft.AspNetCore.Http;

namespace EcoLefty.Application.Common.Images;

public class ImageService : IImageService
{
    public async Task<string?> UploadImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        //var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        var uploadsFolder = @"C:\Users\Giorgi\source\repos\EcoLefty\EcoLefty.Web\wwwroot\uploads\";

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream, cancellationToken);
        }

        return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
    }
}
