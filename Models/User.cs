#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportService.Models
{
    // [Table("User", Schema = "dbo")]
    [Table("User")]
    public partial class User
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(20)]
        public string Telephone { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordSalt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsStaff { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        public string? OTP { get; set; }
        public bool TelephoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        
        
    }
}
