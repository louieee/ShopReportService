using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.crm;

[Table("lead")]
public partial class Lead
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("title")]
    public string Title { get; set; }
    
    [Column("contactid")]
    public int ContactId { get; set; }
    
    [Column("ownerid")]
    public int OwnerId { get; set; }
    
    [Column("source")]
    public string Source { get; set; }
    
    [Column("nurturingstatus")]
    public string NurturingStatus { get; set; }
    
    [Column("isdeal")]
    public bool IsDeal { get; set; }
    
    [Column("company")]
    public string Company { get; set; }
    
    [Column("conversiondate")]
    public DateTime? ConversionDate { get; set; }
    
    [Column("datecreated")]
    public DateTime DateCreated { get; set; }
    
}