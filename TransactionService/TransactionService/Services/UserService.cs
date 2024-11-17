using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TransactionService.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/users/{userId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return user;
    }
}
