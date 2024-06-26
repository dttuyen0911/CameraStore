﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CameraStore.Models
{
    public class Feedback
    {
        [Key]
        public int feedID { get; set; }
        [ForeignKey("Customer")]
        public int customerID { get; set; }
        public virtual Customer? Customer { get; set; }
        [ForeignKey("Product")]
        public int proID { get; set; }
        public virtual Product? Product { get; set; } 
        [ForeignKey("Order")]
        public int orderID { get; set; }
        [JsonIgnore]
        public virtual Order? Order { get; set; }
        public string feedDescription { get; set; }
        public DateTime createDate { get; set; } = DateTime.Now;
        public int StarRating { get; set; }
        [NotMapped]
        public IFormFile? feedImage { get; set; }
        public string? feedUrlImage { get; set; }
    }
}