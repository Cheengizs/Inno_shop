using System.Net.Http.Json;
using Users.Application.Clients;

namespace Users.Infrastructure.Clients;

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UpdateUserStatusAsync(int userId, bool isActive)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"/api/products/internal/user-status/{userId}", isActive);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to update products for user {userId}. Code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling Product Service: {ex.Message}");
        }
    }
}