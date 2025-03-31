namespace EcoLefty.Application.Products.DTOs;

public record CreateProductRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public string ImageUrl { get; set; }
    public IEnumerable<int> CategoryIds { get; set; } = [];
}
