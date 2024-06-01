using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ReportApp.Data.Requests.acccounts;

namespace ReportApp.Data.Models.inventory;

[Keyless]
public class StaffDeliveredProduct{
    
    [Column("staff")]
    public string Staff { get; set; }
    [Column("products_delivered")]
    public int ProductsDelivered { get; set; }
}

[Table("orders_view")]
public class ProductOrders
{
    [Column("id"), Key]
    public int Id { get; set; }
    
    [Column("staffid")]
    public int StaffId { get; set; }
    
    [Column("customer")]
    public string Customer { get; set; }
    
    [Column("age")]
    public int Age { get; set; }
    [Column("gender")]
    public string Gender { get; set; }
    [Column("location")]
    public string Location { get; set; }
    [Column("product_id")]
    public int ProductId { get; set; }
    [Column("product")]
    public string Product { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("delivered")]
    public bool Delivered { get; set; }
    [Column("paid")]
    public bool Paid { get; set; }
    
    [Column("datedelivered")]
    public DateTime? DateDelivered { get; set; }
    
    [Column("datepaid")]
    public DateTime? DatePaid { get; set; }
}

[Keyless]
public class ProductOrderedByAgeView
{
    [Column("age")]
    public int Age { get; set; }
   
    [Column("products_ordered")]
    public int NumberOfOrders { get; set; }
}

[Keyless]
public class ProductOrderedByGenderView
{
    [Column("gender")]
    public string Gender { get; set; }
   
    [Column("products_ordered")]
    public int NumberOfOrders { get; set; }
}

[Keyless]
public class ProductOrderedByLocationView
{
    [Column("location")]
    public string Location { get; set; }
   
    [Column("products_ordered")]
    public int NumberOfOrders { get; set; }
}

[Keyless]
public class ProductOrderedByQuantityView
{
    [Column("product")]
    public string Product{ get; set; }
   
    [Column("products_ordered")]
    public int NumberOfOrders { get; set; }
    
    [Column("total_quantity")]
    public int TotalQuantity { get; set; }
}


[Table("product_speed_report")]
public class ProductSpeedView
{
    [Column("product_id"), Key]
    public int ProductId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("age")]
    public int Age { get; set; }
    
    [Column("qty_sold")]
    public int QtySold { get;set; }
    
    [Column("sell_through_ratio")]
    public float SellThroughRatio { get; set; }
    
    [Column("product_sold_per_day")]
    public int ProductSoldPerDay { get; set; }
    
    [Column("sale_speed")]
    public float SaleSpeed { get; set; }
    
}

[Keyless]
public class ProductsSoldPerYear
{
    [Column("year")]
    public int Year { get; set; }
    
    [Column("products_sold")]
    public int ProductsCount { get; set; }
}

[Keyless]
public class ProductsSoldPerMonth
{
    [Column("year")]
    public int Year { get; set; }
    
    [Column("month")]
    public MonthEnum Month { get; set; }
    
    [Column("products_sold")]
    public int ProductsCount { get; set; }

    public string MonthStr => Month.ToString();
}

[Keyless]
public class ProductsSoldPerDay
{
    [Column("date_sold")]
    public DateOnly DateSold { get; set; }
    
    [Column("products_sold")]
    public int ProductsCount { get; set; }

    
}

[Keyless]
public class ProductsSoldPerHour
{
    [Column("date_sold")]
    public DateOnly DateSold { get; set; }
    
    [Column("hour_sold")]
    public DateOnly HourSold { get; set; }
    
    [Column("products_sold")]
    public int ProductsCount { get; set; }

    
}