using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nine.Data;
using Nine.Models;

namespace Nine.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


//=====================================================================================================//

        // Thêm Item vào giỏ hàng 
        public int AddItemS(int bookId, int qty)
        {
            // Lay ID Người Dùng để tại Giỏ Hàng (Mỗi người dùng có giỏ hàng khác nhau)
            var userId = GetUserId();
            var cart = GetCart(userId);
            if (cart is null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId
                };
                _context.ShoppingCarts.Add(cart);
            }
            _context.SaveChanges();

            //Tạo Giỏ hàng
            var cartItem = _context.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
            if (cartItem is not null)
            {
                cartItem.Quantity += qty;
            }
            else
            {
                var book = _context.Books.Find(bookId);
                cartItem = new CartDetail
                {
                    BookId = bookId,
                    ShoppingCartId = cart.Id,
                    Quantity = qty,
                    UnitPrice = book.Price
                };
                _context.CartDetails.Add(cartItem);
            }
            _context.SaveChanges();


            return bookId;
        }


        public IActionResult AddItem(int bookId, int qty = 1)
        {
            var cartCount = AddItemS(bookId, qty);
            return RedirectToAction("GetUserCart");
        }

//=====================================================================================================//

        // Hiện toàn bộ thông tin đã add vô cart 
        public ShoppingCart GetUserCartS()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userid");
            var shoppingCart = _context.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.Genre)
                                  .Where(a => a.UserId == userId).FirstOrDefault();
            return shoppingCart;

        }


        public IActionResult GetUserCart()
        {
            var cart = GetUserCartS();
            return View(cart);
        }

//=====================================================================================================//


        // Xóa Item trong giỏ hàng  ( nếu còn sản phẩm thì trừ sản phẩm đi 1 và nếu sản phẩm còn 1 thì xóa)
        public int RemoveItemS(int bookId)
        {
            string userId = GetUserId();
            var cart = GetCart(userId);
            var cartItem = _context.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
            if (cartItem.Quantity == 1)
            {
                _context.CartDetails.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = cartItem.Quantity - 1;
            }
            _context.SaveChanges();
            return bookId;
        }


        public IActionResult RemoveItem(int bookId)
        {
            var cartCount = RemoveItemS(bookId);
            return RedirectToAction("GetUserCart");
        }

//=====================================================================================================//

        //Order sản phẩm 
        public bool DoCheckoutS()
        {
            // Khi clickbutton thì dữ liệu CartDetail trong giỏ hàng sẽ chuyển sang order sau đó xóa cái CartDetail

            var userId = GetUserId();
            var cart = GetCart(userId);
            var cartDetail = _context.CartDetails.Where(a => a.ShoppingCartId == cart.Id).ToList();

            var order = new Order
            {
                UserId = userId,
                CreateDate = DateTime.UtcNow,
                OrderStatusId = 1 //
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartDetail)
            {
                var orderDetail = new OrderDetail
                {
                    BookId = item.BookId,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            _context.CartDetails.RemoveRange(cartDetail);
            _context.SaveChanges();
            return true;
        }

   
        public IActionResult CheckOut()
        {
            bool isCheckOut = DoCheckoutS();
            return RedirectToAction("Index", "Home");
        }

//=====================================================================================================//

        
        public ShoppingCart GetCart(string userId)
        {
            var cart = _context.ShoppingCarts.FirstOrDefault(x => x.UserId == userId);
            return cart;
        }

//=====================================================================================================//

        //Lấy ID của User
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
//=====================================================================================================//

    }
}
