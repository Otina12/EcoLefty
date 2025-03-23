namespace EcoLefty.Application.Products.DTOs;

public record UpdateProductRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public string ImageUrl { get; set; }
}