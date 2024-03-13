using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CameraStore.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int proID { get; set; }
        [Required(ErrorMessage = "Name of product is not null")]
        [RegularExpression(@"^[a-zA-Z\s\W]+$", ErrorMessage = "Name must be character")]
        [StringLength(150, ErrorMessage = "String length no more than 150 characters")]
        public string proName { get; set; }
        [Required(ErrorMessage = "Description of product is not null")]
        public string proDescription { get; set; }
        [NotMapped]
        public IFormFile? proImage { get; set; }
        public string? proUrlImage { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        public DateTime proDate { get; set; }
        public int proQuantity { get; set; }
        [Required(ErrorMessage = "Please enter a valid price sale")]
        [Range(0, double.MaxValue, ErrorMessage = "Price sale must be a non-negative value")]
        public decimal? proSale { get; set; } // price sale when sub percent

        [Required(ErrorMessage = "Please enter a status of product")]
        public string proStatus { get; set; } // old or new or sold out
        [Required(ErrorMessage = "Please enter a percent sale of product")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Percent must be a non-negative integer")]
        public string proPercent { get; set; } // percent sale if any
        [Required(ErrorMessage = "Please enter a valid price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value")]
        public decimal proPrice { get; set; } // price original
        [ForeignKey("Supplier")]
        public int supID { get; set; }
        public Supplier? Supplier { get; set; }
        [ForeignKey("Category")]
        public int cateID { get; set; }
        public Category? Category { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> orderdetails { get; set; }
        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}
