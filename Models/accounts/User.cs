#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReportService.Models
{
    // [Table("User", Schema = "dbo")]
    [Table("users")]
    public partial class User
    {
        [Column("id"), Key]
        public int Id { get; set; }
        [Column("firstname"), Required, StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100), Required,Column("lastname") ]
        public string LastName { get; set; }
        
        [Column("email"), EmailAddress]
        public string Email { get; set; }
        
        [Column("customerid")]
        public int? CustomerId { get; set; }
        
        [Column("staffid")]
        public int? StaffId { get; set; }
        
        [Column("gender"), DefaultValue("male")]
        public string Gender { get; set; }
        
        [Column("date_of_birth")]
        public DateOnly DateOfBirth { get; set; }
        
        
    }
}
