namespace EcoLefty.Application.Authentication.DTOs;

public class AuthenticationResultDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}