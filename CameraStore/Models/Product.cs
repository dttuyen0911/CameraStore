﻿using System.ComponentModel.DataAnnotations.Schema;
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
        [Required(ErrorMessage = "Please select a image")]
        public string? proUrlImage { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        public DateTime proDate { get; set; }
        public int? proQuantity { get; set; }
        [Required(ErrorMessage = "Please select a sale")]
        public decimal? proSale { get; set; }
        [Required(ErrorMessage = "Please select a status")]
        public Boolean proStatus { get; set; }
        [Required(ErrorMessage = "Please select a percent")]
        public string? proPercent { get; set; }
		public decimal? proPrice { get; set; }
        [ForeignKey("Supplier")]
        public int supID { get; set; }
        public Supplier? Supplier { get; set; }
        [ForeignKey("Category")]
        public int cateID { get; set; }
        public Category? Category { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<OrderDetail>? orderdetails { get; set; }
        public virtual ICollection<CartDetails>? CartDetails { get; set; }
    }
}
