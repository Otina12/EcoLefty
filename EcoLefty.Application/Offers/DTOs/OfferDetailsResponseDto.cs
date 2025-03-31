using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Common.Enums;

namespace EcoLefty.Application.Offers.DTOs;

public record OfferDetailsResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; } // unit price (discounted) of a product
    public int TotalQuantity { get; set; } // quantity of a product
    public int QuantityAvailable { get; set; } // quantity of products left
    public OfferStatus OfferStatus { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    public CompanyResponseDto Company { get; set; }
    public ProductResponseDto Product { get; set; }
    public IEnumerable<CategoryResponseDto> Categories { get; set; }
}
