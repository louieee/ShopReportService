using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.inventory;

[Table("sale")]
public partial class Sale
{
    [Key, Column("id")]
    public int Id { get; set; }
    
    [Column("paid"), DefaultValue(false)]
    public bool Paid { get; set; }
    [Column("customerid")]
    public int CustomerId { get; set; }
    
    [Column("dateordered")]
    public DateTime DateOrdered { get; set; }
    
    [Column("datepaid")]
    public DateTime? DatePaid { get; set; }
    
    [Column("location")]
    public string Location { get; set; }

    
}