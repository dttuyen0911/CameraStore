using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CameraStore.Models
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int supID { get; set; }
        [Required(ErrorMessage = "Name of supplier is not null")]
        [RegularExpression(@"^[a-zA-Z\s\W]+$", ErrorMessage = "Name must be character")]
        [StringLength(150, ErrorMessage = "String length no more than 150 characters")]
        public string supName { get; set; }
        [Required(ErrorMessage = "Description of supplier is not null")]
        [StringLength(255, ErrorMessage = "String length no more than 255 characters")]
        public string supDescription { get; set; }
        [Required(ErrorMessage = "Address of supplier is not null")]
        [StringLength(255, ErrorMessage = "String length no more than 255 characters")]
        public string supAddress { get; set; }
        [Required(ErrorMessage = "Telephone of supplier is not null")]
        [RegularExpression(@"^\d[0-9]{9}$", ErrorMessage = "Phone numbers that only accept 10 numbers are allowed or telephone is number")]
        public string supTelephone { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
