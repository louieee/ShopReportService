using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.inventory;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("productid")]
    public int ProductId { get; set; }
    
    [Column("saleid")]
    public int SaleId { get; set; }
    
    [Column("staffid")]
    public int? StaffId { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("delivered")]
    public bool Delivered { get; set; }
    
    [Column("datedelivered")]
    public DateTime? DateDelivered { get; set; }

    [Column("datecreated")]
    public DateTime? DateCreated { get; set; }

}