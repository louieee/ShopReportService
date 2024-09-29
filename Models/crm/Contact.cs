using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.crm;

[Table("contact")]
public partial class Contact
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("ownerid")]
    public int OwnerId { get; set; }

    [Column("datecreated")]
    public DateTime DateCreated { get; set; }

    [Column("name")]
    public string Name { get; set; }
}