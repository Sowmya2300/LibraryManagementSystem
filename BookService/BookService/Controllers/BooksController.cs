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

			return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
		}

		// PUT: api/books/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBook(int id, Book book)
		{
			if (id != book.Id)
			{
				return BadRequest();
			}

			_context.Entry(book).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Books.Any(b => b.Id == id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
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

			return NoContent();
		}
	}
}
