using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI2.Data;
using LibraryAPI2.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorBooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorBooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AuthorBooks
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorBook>>> GetAuthorBooks()
        {
            return await _context.AuthorBooks.ToListAsync();
        }

        // GET: api/AuthorBooks/5

        [Authorize(Roles = "Admin, Employee, Manager")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(long id)
        {
            var authorBook = await _context.AuthorBooks.FindAsync(id);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }

        // PUT: api/AuthorBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [Authorize(Roles = "Admin, Employee, Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthorBook(long id, AuthorBook authorBook)
        {
            if (id != authorBook.AuthorsId)
            {
                return BadRequest();
            }

            _context.Entry(authorBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorBookExists(id))
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

        // POST: api/AuthorBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [Authorize(Roles = "Admin, Employee, Manager")]
        [HttpPost]
        public async Task<ActionResult<AuthorBook>> PostAuthorBook(AuthorBook authorBook)
        {
            _context.AuthorBooks.Add(authorBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorBookExists(authorBook.AuthorsId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthorBook", new { id = authorBook.AuthorsId }, authorBook);
        }

        // DELETE: api/AuthorBooks/5

        [Authorize(Roles = "Admin, Employee, Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthorBook(long id)
        {
            var authorBook = await _context.AuthorBooks.FindAsync(id);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBooks.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorBookExists(long id)
        {
            return _context.AuthorBooks.Any(e => e.AuthorsId == id);
        }
    }
}
