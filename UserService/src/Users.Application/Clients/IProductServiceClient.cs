namespace Users.Application.Clients;

public interface IProductServiceClient
{
    Task UpdateUserStatusAsync(int userId, bool isActive);
}