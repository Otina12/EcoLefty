using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Application.Products.DTOs;

namespace EcoLefty.Application.Companies.DTOs;

public record CompanyDetailsResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string LogoUrl { get; set; }
    public decimal Balance { get; set; }
    public bool IsApproved { get; set; }
    public IEnumerable<ProductDetailsResponseDto> Products { get; set; } = [];
    public IEnumerable<OfferDetailsResponseDto> Offers { get; set; } = [];
}
