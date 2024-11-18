using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TransactionService.Models;

public class BookService : IBookService
{
    private readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Book> GetBookByIdAsync(int bookId)
    {
        var response = await _httpClient.GetAsync($"/api/books/{bookId}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Book>();
        }
        return null;
    }
}
