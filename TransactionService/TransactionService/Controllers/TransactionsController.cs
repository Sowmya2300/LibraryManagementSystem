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

        public TransactionsController(TransactionDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            // Fetch user data from UserService based on UserId in the transaction
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null) return NotFound($"User with ID {transaction.UserId} not found in UserService.");

            // Optionally, include user details in the response
            return Ok(new
            {
                transaction.TransactionId,
                transaction.BookId,
                transaction.BorrowedDate,
                transaction.ReturnedDate,
                transaction.Status,
                User = user // User details fetched from UserService
            });
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<object>> CreateTransaction(Transaction transaction)
        {
            // Validate user with UserService
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            // Add transaction to the database
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Return the transaction along with user details
            var response = new
            {
                Transaction = transaction,
                UserDetails = user
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.TransactionId }, response);
        }


        // PUT: api/transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            if (id != transaction.TransactionId) return BadRequest();

            // Validate the UserId with UserService before updating the transaction
            var user = await _userService.GetUserByIdAsync(transaction.UserId);
            if (user == null) return BadRequest($"Invalid UserId: {transaction.UserId}. User not found.");

            _context.Entry(transaction).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.TransactionId == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
