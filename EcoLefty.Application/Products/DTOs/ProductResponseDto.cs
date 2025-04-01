using EcoLefty.Application.Companies.DTOs;

namespace EcoLefty.Application.Products.DTOs;

public record ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public string ImageUrl { get; set; }
    public CompanyResponseDto Company { get; set; }
}
