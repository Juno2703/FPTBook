using BookShoppingCartMvcUI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nine.Data;
using Nine.Models;

namespace Nine.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor
            , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

 //=====================================================================================================//

        //Show các đơn hàng đã order
        public IEnumerable<Order> UserOrdersS()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not Login");
            var orders = _context.Orders
                            .Include(x => x.OrderStatus)
                            .Include(x => x.OrderDetail)
                            .ThenInclude(x => x.Book)
                            .ThenInclude(x => x.Genre)
                            .Where(a => a.UserId == userId).ToList();
            return orders;
        }


        public IActionResult UserOrders()
        {
            var order = UserOrdersS();
            return View(order);
        }

//=====================================================================================================//

        public IActionResult GetOrder()
        {
            var applicationDbContext = _context.Orders.Include(o => o.OrderStatus);
            return View(applicationDbContext.ToList());
        }

 //=====================================================================================================//

        public IActionResult ShipOrder(int? id)
        {
            var order = _context.Orders.Find(id);
            if (order.OrderStatusId != null)
            {
                order.OrderStatusId = 2;
            }
            _context.SaveChanges();
            return RedirectToAction("GetOrder");
        }

//=====================================================================================================//

        public IActionResult CancelOrder(int? id)
        {
            var order = _context.Orders.Find(id);
            if (order.OrderStatusId != null)
            {
                order.OrderStatusId = 3;
            }
            _context.SaveChanges();
            return RedirectToAction("UserOrders");
        }

//=====================================================================================================//
        
       public async Task<ActionResult> Delete(int id)
        {
            Order order = _context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction("GetOrder");
            }
            else
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return RedirectToAction("GetOrder");
            }
        }

//=====================================================================================================//
        
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
