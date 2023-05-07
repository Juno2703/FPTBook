
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nine.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string BookName { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [Required, MaxLength(13)]
        public string ISBN { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Date Pulished")]
        public DateTime DatePulished { get; set; }

        [Required, DataType(DataType.Currency)]
        public int Price { get; set; }

        [Required]
        public string Author { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
        [Required]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public List<OrderDetail>? OrderDetail { get; set; }
        public List<CartDetail>? CartDetail { get; set; }
        [NotMapped]
        public string? GenreName { get; set; }

    }
}
