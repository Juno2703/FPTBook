using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nine.Data;
using Nine.Models;

namespace Nine.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

//=====================================================================================================//

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books.Include(b => b.Genre);
            return View(await applicationDbContext.ToListAsync());
        }

//=====================================================================================================//

        [Authorize(Roles = "Owner")]
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

//=====================================================================================================//

        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres.Where(c => c.GenreStatus == "accepted").ToList(), "Id", "GenreName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookName,Description,ISBN,DatePulished,Price,Author,ImageUrl,GenreId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "GenreName", book.GenreId);
            return View(book);
        }

//=====================================================================================================//

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres.Where(c => c.GenreStatus == "accepted").ToList(), "Id", "GenreName");
            
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookName,Description,ISBN,DatePulished,Price,Author,ImageUrl,GenreId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres.Where(c => c.GenreStatus == "processed").ToList(), "Id", "GenreName");
            return View(book);
        }

//=====================================================================================================//

        // GET: Books/Delete/5
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Delete(int id)
        {
            Book book = _context.Books.Find(id);
            if (book == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
