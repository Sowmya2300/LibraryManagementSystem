using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
//using LibraryManagementSystem.Models;  // Use your model namespace
using System.Linq;
using BookService.Models;
using BookService.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BooksController : ControllerBase
	{
		private readonly BookDbContext _context;

		public BooksController(BookDbContext context)
		{
			_context = context;
		}

		// GET: api/books
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
		{
			return await _context.Books.ToListAsync();
		}

		// GET: api/books/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Book>> GetUser(int id)
		{
			var Book = await _context.Books.FindAsync(id);

			if (Book == null)
			{
				return NotFound();
			}

			return Book;
		}

		// POST: api/books
		[HttpPost]
		public async Task<ActionResult<Book>> CreateBook(Book book)
		{
			_context.Books.Add(book);
			await _context.SaveChangesAsync();

            var response = new
            {
                status = "success",
                message = "Book added successfully.",
                data = book
            };

            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
		}

        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest(new { status = "error", message = "Book ID mismatch." });
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return NotFound(new { status = "error", message = "Book not found." });
            }

            // Update the book details
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.YearPublished = book.YearPublished;
            existingBook.Genre = book.Genre;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(b => b.Id == id))
                {
                    return NotFound(new { status = "error", message = "Book not found during update." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
                status = "success",
                message = "Book updated successfully.",
                data = existingBook
            });
        }


        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBook(int id)
		{
			var book = await _context.Books.FindAsync(id);
			if (book == null)
			{
				return NotFound();
			}

			_context.Books.Remove(book);
			await _context.SaveChangesAsync();

            var response = new
            {
                status = "success",
                message = "Book deleted successfully.",
            };

            return NoContent();
		}
	}
}
