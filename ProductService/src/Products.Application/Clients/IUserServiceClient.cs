namespace Products.Application.Clients;

public interface IUserServiceClient
{
    Task<bool> ExistsAsync(int userId);
}