namespace EcoLefty.Application.ApplicationUsers.DTOs;

public record ApplicationUserResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
    public decimal Balance { get; set; }
}