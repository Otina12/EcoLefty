using Microsoft.AspNetCore.Http;

namespace EcoLefty.Application.Products.DTOs;

public record CreateProductRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public IFormFile? ImageFile { get; set; }
    public List<int> CategoryIds { get; set; } = [];
}
