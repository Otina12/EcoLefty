namespace EcoLefty.Application.Companies.DTOs;

public record CreateCompanyRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string? LogoUrl { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
