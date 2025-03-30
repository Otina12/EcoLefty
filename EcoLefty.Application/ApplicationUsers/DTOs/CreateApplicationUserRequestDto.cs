namespace EcoLefty.Application.ApplicationUsers.DTOs;

public record CreateApplicationUserRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
    public int BirthDay { get; set; }
    public int BirthMonth { get; set; }
    public int BirthYear { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
