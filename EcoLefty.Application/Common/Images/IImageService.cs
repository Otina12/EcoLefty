using Microsoft.AspNetCore.Http;

namespace EcoLefty.Application.Common.Images;

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile imageFile, CancellationToken cancellationToken = default);
}