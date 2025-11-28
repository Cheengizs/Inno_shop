using System.Net.Http.Json;
using Products.Application.Clients;
using Shared.Contracts;

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

    public async Task<bool> IsEmailConfirmedAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}/status");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var status = await response.Content.ReadFromJsonAsync<UserStatusDto>();
            return status?.EmailConfirmed ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error communicating with User Service: {ex.Message}");
            return false;
        }
    }
}