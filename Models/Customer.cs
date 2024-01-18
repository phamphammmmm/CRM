using System.ComponentModel.DataAnnotations;

namespace Customer_Relationship_Managament.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public bool? Gender { get; set; } = false;
        [Required]
        public DateTime? DateOfBirth { get; set; } = DateTime.Now;
        public string QRCodeURL { get; set; } = string.Empty;
    }
}
