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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Diagnostics;

namespace LibraryAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public EmployeesController(AppDbContext context, IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Employees
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            if (employee.Title == "Manager")
            {
                var user = await _userManager.FindByIdAsync(employee.Id);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, "Manager");
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }
            if (employee.Title == "Librarian")
            {
                var user = await _userManager.FindByIdAsync(employee.Id);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, "Librarian");
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }
            if (employee.Title == "Intern")
            {
                var user = await _userManager.FindByIdAsync(employee.Id);
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, "Intern");
                   // employee.Insurance = false;
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }



        // DELETE: api/Employees/5
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            employee.AppUser.IsDeleted = true;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user.IsDeleted == true) { return NotFound(); }
            

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                // Get user roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Create the JWT claims
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

                // Add role claims
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(3),
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

        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPost("Enter the returned book")]
        public async Task<IActionResult> ReturnedBook(string MemberId, int BookCopyId)
        {
            var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(x => x.Id == BookCopyId);
            var rentedBook = await _context.RentedBooks.FirstOrDefaultAsync(x => x.MemberId == MemberId && x.CopyId == BookCopyId);
            var bookStock = await _context.BookStocks.FirstOrDefaultAsync(x => x.BookId == bookCopy.BookId);

            if (rentedBook != null && bookCopy != null)
            {
                rentedBook.IsReturned = true;
                bookCopy.IsAvailable = true;
                bookStock.StockCount++;
                rentedBook.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPost("Duplicate copies (increase book's stock)")]
        public async Task<IActionResult> CopyDuplication(int BookId, int HowMany)
        {
            var book = _context.Books.Find(BookId);
            var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(x => x.BookId == BookId);
            var stock = await _context.BookStocks.FindAsync(BookId);

            if (bookCopy != null && stock != null)
            {
                for (int i = 0; i < HowMany; i++)
                {
                    var copy = new BookCopy
                    {
                        BookId = BookId,
                        IsAvailable = true
                    };
                    _context.BookCopies.Add(copy);
                    stock.StockCount++;

                }
                book.HowManyCopy += HowMany;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();

        }
        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPost("Enter donator info (for those who aren't a member in our system)")]
        public async Task<IActionResult> DonatorInfo(string Name, string Email)
        {
            // sistemimizde kayıtlı olan üyeler kendi hesaplarından donator olabildiği için bu metod üye olmak istemeyen şahıslar,şirketler için

            var donator = new Donator
            {
                Name = Name,
                Email = Email
            };
            _context.Donators.Add(donator);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPost("Enter donated books / if the book doesn't exist / if error 400 delete the whole book info and retry")]
        public async Task<IActionResult> DonatedBooks(string DonatorEmail, string BookTitle, string Language, string PublisherName, int HowManyCopy, Book? book)
        {
            // isim,yayımcı,dil uyuşuyorsa o kitabın kopyasını oluştursun değilse yeni kitabı sisteme elle girilsin
            var findPublisher = await  _context.Publishers!.FirstOrDefaultAsync(x => x.Name == PublisherName);
            var findLanguage = await _context.Languages.FirstOrDefaultAsync(x => x.Name == Language);
            var donatorName = await _context.Donators.FirstOrDefaultAsync(x=> x.Email == DonatorEmail);

            var findDonator = await _context.Donators.FindAsync(donatorName.Id);
            if (findDonator == null) { return NotFound("Donator is not registered."); }
            var findBook = await _context.Books!.FirstOrDefaultAsync(x => x.Title == BookTitle && x.PublisherId == findPublisher!.Id && x.LanguageCode == findLanguage.Code); // null dönüyor buna bakmam lazım
            // propları ayrı ayrı find edip breakpointle test edebilirim
            if (findBook != null)
            {
                for (int i = 0; i < HowManyCopy; i++)
                {
                    var donatedBookCopy = new BookCopy
                    {
                        BookId = findBook.Id,
                        DonatorId = findDonator.Id,
                        IsAvailable = true
                    };

                    _context.BookCopies.Add(donatedBookCopy);
                }
                await _context.SaveChangesAsync();    

                var bookStok = await _context.BookStocks.FindAsync(findBook.Id);
                bookStok.StockCount += HowManyCopy;
                _context.BookStocks.Update(bookStok);

                await _context.SaveChangesAsync();


                if (findDonator.UserId != null)
                {

                    var donatedBook = new DonatedBook
                    {
                        BookId = findBook.Id,
                        DonatorId = findDonator.Id,
                        UserId = findDonator.UserId,
                        HowMany = HowManyCopy
                    };
                    _context.DonatedBooks.Add(donatedBook);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                if (findDonator.UserId == null)
                {
                    var donatedBook = new DonatedBook
                    {
                        BookId = findBook.Id,
                        DonatorId = findDonator.Id,
                        UserId = null
                    };
                    _context.DonatedBooks.Add(donatedBook);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

            }
            // eğer kitap önceden kayıtlı değilse
            book.HowManyCopy = HowManyCopy; // request body'deki field'ı burda verdiğimiz değere atadık. Nedeni ise olmayan kitabı kaydeceği için request body'ydekine kaç değer yazdığımız farketmesin.
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            for (int i = 0; i < HowManyCopy; i++)   // kitap sistemimizde olmadığı için bizim metodda verdiğimiz HowManyCopy parametresi kadar kaydettik
                                                    // request body'de Book'taki HowManyCopy field'ına 0 dahi yazsaydık bir şey farketmeyecek
            {
                var donatedBookCopy = new BookCopy
                {
                    BookId = book.Id,
                    DonatorId = findDonator.Id,
                    IsAvailable = true
                };
                
                _context.BookCopies.Add(donatedBookCopy);
                await _context.SaveChangesAsync();
            }
            var bookStock = new BookStock();
            bookStock.BookId = book.Id;
            bookStock.StockCount = book.HowManyCopy;
            _context.BookStocks.Add(bookStock);
            await _context.SaveChangesAsync();

            if (findDonator.UserId != null)
            {

                var donatedBook = new DonatedBook
                {
                    BookId = book.Id,
                    DonatorId = findDonator.Id,
                    UserId = findDonator.UserId,
                    HowMany = HowManyCopy
                };
                _context.DonatedBooks.Add(donatedBook);
                await _context.SaveChangesAsync();
                return Ok();
            }
            if (findDonator.UserId == null)
            {
                var donatedBook = new DonatedBook
                {
                    BookId = book.Id,
                    DonatorId = findDonator.Id,
                    UserId = null
                };
                _context.DonatedBooks.Add(donatedBook);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return Ok();
        }



        [Authorize(Roles = "Admin,Manager,Librarian")]
        [HttpPost("Member Penalties")]
        public async Task<IActionResult> Penalty(string MemberId)
        {
            var userRents = await _context.RentedBooks.FirstOrDefaultAsync(x => x.MemberId == MemberId);
            int penaltyPerDay = 4;

            if (userRents != null )
            {
                if (userRents.ReturnDate > userRents.MustReturnDate)
                {
                    var daysOverdue = (userRents.ReturnDate.Value - userRents.MustReturnDate).Days;
                    var totalPenalty = daysOverdue * penaltyPerDay;

                    var penalties = new Penalty
                    {
                        MemberId = MemberId,
                        BookCopyIdNotReturned = userRents.CopyId,
                        TotalPenalty = totalPenalty

                    };
                    _context.Penalties.Add(penalties);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();

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
