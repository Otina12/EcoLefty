namespace EcoLefty.Application.ApplicationUsers.DTOs;

public record UpdateApplicationUserRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
}