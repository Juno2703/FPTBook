using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nine.Data;
using Nine.Models;
using System.Diagnostics;

namespace Nine.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

//=====================================================================================================//
        
        //Show Trang home
        public async Task<IActionResult> Index(string search)
        {
			var books = _context.Books.Select(a => a);
			if (!string.IsNullOrEmpty(search))
			{
				books = books.Where(a => a.BookName.Contains(search) || a.Author.Contains(search));
			}
			return View(await books.ToListAsync());
		}

//=====================================================================================================//

        // Show thông tin chi tiết của sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        
    }
}