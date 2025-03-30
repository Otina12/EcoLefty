namespace EcoLefty.Application.Authentication.Tokens.DTOs;

public record TokenResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
