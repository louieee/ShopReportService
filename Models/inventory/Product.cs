using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ReportApp.Data.Models.inventory;

[Table("product")]
public partial class Product
{
    
    [Column("id"), Key]
    public int Id  {get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("brand")]
    public string Brand { get; set; }
    
    [Column("inventory")]
    public string Inventory { get; set; }
    
    [Column("price")]
    public float Price { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("date_added")]
    public DateTime DateAdded { get; set; }
}