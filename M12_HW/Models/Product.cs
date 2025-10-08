using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M12_HW.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(1024)]
        public string? Description { get; set; }

        [NotMapped]
        public IFormFile? ImageData { get; set; }
        public string? ImageType { get; set; }
        public byte[]? ImageFile { get; set; }

        public override string ToString() => $"Id: {Id}, Name: {Name}, Price: {Price}, Description: {Description}";
    }
}
