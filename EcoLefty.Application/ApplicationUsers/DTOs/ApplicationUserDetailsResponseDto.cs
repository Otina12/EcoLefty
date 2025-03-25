using EcoLefty.Application.Categories.DTOs;

namespace EcoLefty.Application.ApplicationUsers.DTOs;

public record ApplicationUserDetailsResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
    public decimal Balance { get; set; }
    public IEnumerable<CategoryResponseDto> FollowedCategories { get; set; } = [];

}
