using System.ComponentModel.DataAnnotations;

namespace CameraStore.Models
{
    public class Role
    {
        [Key]
        public int roleID { get; set; }
        [Required(ErrorMessage = "Name of role is not null")]
        [RegularExpression(@"^[a-zA-Z\s\W]+$", ErrorMessage = "Name must be character")]
        [StringLength(150, ErrorMessage = "String length no more than 150 characters")]
        public string name { get; set; }
        [Required(ErrorMessage = "Description of role is not null")]
        [RegularExpression(@"^[a-zA-Z\s\W]+$", ErrorMessage = "Description must be character")]
        [StringLength(150, ErrorMessage = "String length no more than 150 characters")]
        public string description { get; set; }
        public virtual ICollection<Customer>? Customers { get; set; }
    }
}