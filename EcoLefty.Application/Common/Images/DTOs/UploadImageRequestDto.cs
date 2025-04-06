using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EcoLefty.Application.Common.Images.DTOs;

public class UploadImageRequest
{
    [Required]
    public IFormFile ImageFile { get; set; }
}
