namespace EcoLefty.Application.Companies.DTOs;

public record CompanyResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string LogoUrl { get; set; }
    public decimal Balance { get; set; }
    public bool IsApproved { get; set; }
}
