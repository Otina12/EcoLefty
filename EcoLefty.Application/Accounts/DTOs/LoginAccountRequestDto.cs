namespace EcoLefty.Application.Accounts.DTOs;

public record LoginAccountRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
