﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CameraStore.Models
{
    public class CartDetails
    {
        [ForeignKey("Cart")]
        public int? cartID { get; set; }
        public virtual Cart? Cart { get; set; }

        [ForeignKey("Product")]
        public int? proID { get; set; }
        public virtual Product? Product { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
