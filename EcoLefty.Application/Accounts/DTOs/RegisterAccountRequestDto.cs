namespace EcoLefty.Application.Accounts.DTOs;

public record RegisterAccountRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}