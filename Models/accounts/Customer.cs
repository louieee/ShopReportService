using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportService.Models;

[Table("customer")]
public class Customer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("datejoined")]
    public DateTime DateJoined { get; set; }
}