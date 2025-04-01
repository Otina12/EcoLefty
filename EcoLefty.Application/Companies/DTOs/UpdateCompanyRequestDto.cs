using Microsoft.AspNetCore.Http;

namespace EcoLefty.Application.Companies.DTOs;

public record UpdateCompanyRequestDto
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string? LogoUrl { get; set; }
    public IFormFile? LogoFile { get; set; }
}
