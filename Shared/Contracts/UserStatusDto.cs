namespace Shared.Contracts;

public class UserStatusDto
{
    public int UserId { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
}