using System.Threading.Tasks;
using TransactionService.Models;

public interface IBookService
{
    Task<Book> GetBookByIdAsync(int bookId);
}
