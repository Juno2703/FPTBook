using BookShoppingCartMvcUI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nine.Data;
using Nine.Models;

namespace Nine.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;           
        }
        
//=====================================================================================================//
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowUser()
        {
            var user = await (from users in _context.Users
                              join UserRole in _context.UserRoles
                              on users.Id equals UserRole.UserId
                              join role in _context.Roles
                              on UserRole.RoleId equals role.Id
                              where role.Name == "User"
                              select users).ToListAsync();
            return View(user);
        }

//=====================================================================================================//

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowOwner()
        {
            var owner = await (from users in _context.Users
                               join UserRole in _context.UserRoles
                               on users.Id equals UserRole.UserId
                               join role in _context.Roles
                               on UserRole.RoleId equals role.Id
                               where role.Name == "Owner"
                               select users).ToListAsync();
            return View(owner);
        }

//=====================================================================================================//

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterOwner()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterOwner(Owner owners)
        {
            if (ModelState.IsValid)
            {
                var owner = new ApplicationUser
                {
                    UserName = owners.Email,
                    Email = owners.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(owner, owners.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(owner, Roles.Owner.ToString());
                    return RedirectToAction("ShowOwner");
                }
                else
                {
                    TempData["Fail"] = "RegisterOwner Fail!";
                    return RedirectToAction("RegisterOwner");
                }
            }
            return RedirectToAction("RegisterOwner");
        }

//=====================================================================================================//
        // Show toàn bộ Genre
        [Authorize(Roles = "Admin")]
        public IActionResult ManageGenre()
        {
            var category = _context.Genres.ToList();
            return View(category);
        }

//=====================================================================================================//

        // click button đổi status Genre
        [Authorize(Roles = "Admin")]
        public IActionResult Accept(int id)
        {
            Genre genre = _context.Genres.Find(id);
            if (genre == null)
            {
                return RedirectToAction("RequestCategory");
            }
            else
            {
                genre.GenreStatus = "accepted";
                _context.Genres.Update(genre);
                _context.SaveChanges();
                return RedirectToAction("ManageGenre");
            }

        }

//=====================================================================================================//
        //Delete Genre
        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.Find(id);
            if (genre == null)
            {
                return RedirectToAction("ManageGenre");
            }
            else
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
                return RedirectToAction("ManageGenre");
            }
        }

//=====================================================================================================//
        
		public async Task<IActionResult> DeleteUser(string id)
		{
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return RedirectToAction("ShowUser");

			}
			else
            {
                var result = await _userManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("ShowUser");
				}  
                return RedirectToAction("ShowUser");
			}
		}

//=====================================================================================================//

        public async Task<IActionResult> DeleteOwner(string id)
		{
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return RedirectToAction("ShowOwner");

			}
			else
            {
                var result = await _userManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                    return RedirectToAction("ShowOwner");
				}  
                return RedirectToAction("ShowOwner");
			}
		}


		

		


	}
}
