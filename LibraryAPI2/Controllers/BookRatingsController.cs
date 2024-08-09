using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI2.Data;
using LibraryAPI2.Models;

namespace LibraryAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookRatingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookRatingsController(AppDbContext context)
        {
            _context = context;
        }

       
        // GET: api/BookRatings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookRating>> GetBookRating(int id)
        {
            var bookRating = await _context.BookRatings.FindAsync(id);

            if (bookRating == null)
            {
                return NotFound();
            }

            return bookRating;
        }

        
    }
}
