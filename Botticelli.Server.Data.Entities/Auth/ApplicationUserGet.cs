namespace Botticelli.Server.Data.Entities.Auth;

public class ApplicationUserGet
{
    public ICollection<Guid>? UserRoleIds;
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}