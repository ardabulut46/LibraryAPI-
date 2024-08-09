using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI2.Data;
using LibraryAPI2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Runtime.InteropServices;

namespace LibraryAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public MembersController(AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;

        }

        // GET: api/Members
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members.ToListAsync();
        }

        // GET: api/Members/5
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(string id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            if (member.AppUser == null)
            {
                return BadRequest("AppUser is required.");
            }

            var createUserResult = await _userManager.CreateAsync(member.AppUser, member.AppUser.Password);
            if (!createUserResult.Succeeded)
            {
                return BadRequest(createUserResult.Errors);
            }

            member.Id = member.AppUser.Id;

            _context.Members.Add(member);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }
      
        // DELETE: api/Members/5
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var member = await _context.Members
                               .Include(m => m.AppUser)
                               .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            if (member.AppUser != null)
            {
                member.AppUser.IsDeleted = true;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(string id)
        {
            return _context.Members.Any(e => e.Id == id);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);


            if (user!.IsDeleted == true)
            {
                if(user.DeletedDate.Value.AddDays(30) <= DateTime.Now)
                {
                    return NotFound();
                }
                user.IsDeleted = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                // Create the JWT
                var authClaims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id ),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }


        [Authorize]
        [HttpPost("RentBook/{BookId}")]
        public async Task<ActionResult> Rent(int BookId)
        {
            var isDeleted = await _context.Books.FirstOrDefaultAsync(x => x.Id == BookId && x.IsDeleted == true);
            // eğer yetkili kişiler kitabı sildiyse( yani isdeleted true ise) kitap kiralanamaz
            if (isDeleted != null) { return NotFound(); }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var bookCopy = await _context.BookCopies!.FirstOrDefaultAsync(x => x.IsAvailable == true && x.BookId == BookId);
            var hasRented = await _context.RentedBooks.FirstOrDefaultAsync(x => x.AppUser.Id == userId && x.BookId == BookId);
            var bookStock = await _context.BookStocks.FindAsync(BookId);
            


            if (bookCopy != null && hasRented == null )
            {
                var rentedBook = new RentedBook
                {
                    MemberId = userId,
                    BookId = BookId,
                    CopyId = bookCopy.Id,
                    RentDate = DateTime.Now
                };
                _context.RentedBooks.Add(rentedBook);
                bookCopy.IsAvailable = false;
                bookStock.StockCount--;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
       
        [Authorize]
        [HttpPost("Rate book")]
        public async Task<IActionResult> Rate(int BookId,float Rating)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rentedBook = await _context.RentedBooks.FirstOrDefaultAsync(x=> x.MemberId == userId && x.BookId== BookId && x.IsReturned == true);

            if (rentedBook != null) 
            {
                var book =await _context.Books.FindAsync(BookId);
                var bookRating = await _context.BookRatings.FindAsync(BookId);
                if (bookRating == null)
                {

                    bookRating = new BookRating
                    {
                        BookId = BookId,
                        RatingSum = Rating,
                        RatingCount = 1
                    };
                    await _context.BookRatings!.AddAsync(bookRating);
                    
                }
                else
                {
                    bookRating.RatingSum += Rating;
                    bookRating.RatingCount++;
                    _context.BookRatings.Update(bookRating);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }
        [Authorize]
        [HttpPost("Become donator")]
        public async Task<IActionResult> BecomeDonator()
        {
            // Tek seferde 3'ten fazla kitap bağışlayabilirse Donator olsun.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);


            var donator = new Donator
            {
                Name = user.FullName,
                UserId = userId,
                Email = user.Email
            };
            _context.Donators.Add(donator);
            await _context.SaveChangesAsync();
            return Ok("You're now donator. Please come to our library with the books you want to donate.");
            
        }
        

        [Authorize]
        [HttpPost("Deactivate your account")]
        public async Task<IActionResult> Deactivate(string Password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var member = await _context.Members.Include(x=> x.AppUser).FirstOrDefaultAsync(x=> x.Id == userId);
            if (member == null) { return NotFound("Member not found."); }

            var appUser = member.AppUser;
            if (appUser == null) { return NotFound("AppUser not found."); }

            var passwordHasher = new PasswordHasher<AppUser>();
            

            var verificationResult = passwordHasher.VerifyHashedPassword(appUser,appUser.PasswordHash,Password);


            if (verificationResult == PasswordVerificationResult.Success)
            {
                appUser.IsDeleted = true;
                appUser.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                await  _signInManager.SignOutAsync();
                return Ok("If you want to activate your account you have to login in 30 days. Else it'll be deleted.");
            }

            return BadRequest();
        }


        [Authorize]
        [HttpPost("Logout")]
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }


    }
        
}