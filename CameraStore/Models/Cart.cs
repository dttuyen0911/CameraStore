using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CameraStore.Models
{
    public class Cart
    {
        [Key]
        public int cartID { get; set; }
        [ForeignKey("Customer")]
        public int customerID { get; set; }
        public virtual Customer? Customer { get; set; }
        public int cartQuantityTotal { get; set; }
        public decimal cartPriceTotal { get; set; } 
        public DateTime timeStamp { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}   
