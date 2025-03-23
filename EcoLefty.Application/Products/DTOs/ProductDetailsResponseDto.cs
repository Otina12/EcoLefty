using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers.DTOs;

namespace EcoLefty.Application.Products.DTOs;

public class ProductDetailsResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public string ImageUrl { get; set; }
    public CompanyResponseDto Company { get; set; }
    public IEnumerable<CategoryResponseDto> Categories { get; set; } = [];
    public IEnumerable<OfferResponseDto> Offers { get; set; } = [];
}
