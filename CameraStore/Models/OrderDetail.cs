using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CameraStore.Models
{
    public class OrderDetail
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Order")]
        public int orderID { get; set; }
        public virtual Order? Order { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Product")]
        public int proID { get; set; }
        public virtual Product? Product { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }

    }
}