namespace EcoLefty.Domain.Contracts;

public interface ICurrentUserContext
{
    string? UserId { get; }
    string? UserRole { get; }
    bool IsInRole(string roleName);
}
