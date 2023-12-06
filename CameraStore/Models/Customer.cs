using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CameraStore.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int customerID { get; set; }
        [Required(ErrorMessage = "Full name of account is not null")]
        [RegularExpression(@"^[a-zA-Z\s\W]+$", ErrorMessage = "Fullname must be character")]
        [StringLength(150, ErrorMessage = "String length no more than 150 characters")]
        public string? fullname { get; set; }
        [Required(ErrorMessage = "Email of account is not null")]
        public string? email {  get; set; }
        [Required(ErrorMessage = "Password of account is not null")]
        public string? password { get; set; }
        [Required(ErrorMessage = "Telephone of account is not null")]
        public string? telephone { get; set; }
        [Required(ErrorMessage = "Address of account is not null")]
        public DateTime? birthday { get; set; }
        [Required]
        [ForeignKey("Role")]
        public int roleID { get; set; }
        public Role? Role { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
