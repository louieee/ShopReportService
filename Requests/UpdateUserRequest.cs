using System.ComponentModel.DataAnnotations;

namespace ReportService.Requests
{
    public class UpdateUserRequest
    {
        [StringLength(50)]
        public string Id { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
    }
}
