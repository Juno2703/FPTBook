using System.ComponentModel.DataAnnotations;

namespace Nine.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
