using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Services;

namespace TransactionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionDbContext _context;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;

        public TransactionsController(TransactionDbContext context, IUserService userService, IBookService bookService)
        {
            _context = context;
            _userService = userService;
            _bookService = bookService;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound("Transaction not found.");

            // Fetch user data from UserService based on UserId in the transaction
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null) return NotFound($"User with ID {transaction.UserId} not found in UserService.");

            // Fetch book data from BookService based on BookId in the transaction
            var book = await _bookService.GetBookByIdAsync(transaction.BookId);
            if (book == null) return NotFound($"Book with ID {transaction.BookId} not found in BookService.");

            // Return transaction details along with user and book information
            return Ok(new
            {
                transaction.TransactionId,
                transaction.BookId,
                transaction.BorrowedDate,
                transaction.ReturnedDate,
                transaction.Status,
                UserDetails = user,   // User details fetched from UserService
                BookDetails = book    // Book details fetched from BookService
            });
        }


        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<object>> CreateTransaction(Transaction transaction)
        {
            // Initialize validation flags
            bool isUserValid = true;
            bool isBookValid = true;

            // Validate user
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null) isUserValid = false;

            // Validate book
            var book = await _bookService.GetBookByIdAsync(transaction.BookId);
            if (book == null) isBookValid = false;

            // Handle validation results
            if (!isUserValid && !isBookValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Both user and book do not exist."
                });
            }
            else if (!isUserValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "User does not exist."
                });
            }
            else if (!isBookValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Book does not exist."
                });
            }

            // Add transaction
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Return transaction along with user and book details
            var response = new
            {
                status = "success",
                message = "Transaction created successfully.",
                Transaction = transaction,
                UserDetails = user,
                BookDetails = book
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, response);
        }


        // PUT: api/transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Transaction ID mismatch."
                });
            }

            // Initialize validation flags
            bool isUserValid = true;
            bool isBookValid = true;

            // Validate user
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null) isUserValid = false;

            // Validate book
            var book = await _bookService.GetBookByIdAsync(transaction.BookId);
            if (book == null) isBookValid = false;

            // Handle validation results
            if (!isUserValid && !isBookValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Both user and book do not exist."
                });
            }
            else if (!isUserValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "User does not exist."
                });
            }
            else if (!isBookValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Book does not exist."
                });
            }

            // Update transaction
            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.TransactionId == id))
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = "Transaction not found."
                    });
                }
                throw;
            }

            // Return updated transaction along with user and book details
            var response = new
            {
                status = "success",
                message = "Transaction updated successfully.",
                Transaction = transaction,
                UserDetails = user,
                BookDetails = book
            };

            return Ok(response);
        }



        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Transaction not found."
                });
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = "Transaction deleted successfully."
            });
        }

    }
}
