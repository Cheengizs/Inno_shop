using Products.Application.Clients;

namespace Products.Infrastructure.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}");
        return response.IsSuccessStatusCode;
    }
}