﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CameraStore.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orderID { get; set; }
        public DateTime orderDate { get; set; } = DateTime.Now;
        public DateTime orderDelivery { get; set; }
        [Display(Name = "Order Status")]
        public bool orderStatus { get; set; }
        public bool IsShipped { get; set; }
        public bool IsPayment { get; set; }
        public bool IsDelivered { get; set; }
        [NotMapped]
        public string OrderStatusString
        {
            get
            {
                return orderStatus ? "Confirmed" : "Pending";
            }
        }
        [Required(ErrorMessage = "Telephone of order is not null")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone numbers that only accept 10 numbers are allowed")]
        [StringLength(50, MinimumLength = 3)]
        public string orderPhone { get; set; }
        [Required(ErrorMessage = "Address of order is not null")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "String length no more than 255 characters")]
        public string orderAddress { get; set; }
        public string orderFullname { get; set; }
        public string paymentMethod { get; set; }
        public decimal totalAmount { get; set; }

        [ForeignKey("Customer")]
        public int customerID { get; set; }
        public Customer? Customer { get; set; }
        public virtual ICollection<OrderDetail>? orderdetails { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }

    }
}