using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.communication;

[Table("groups")]
public partial class Group
{
    [Key]
    [Column("id")]
    public string Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("type")]
    public string Type { get; set; }
    [Column("creatorid")]
    public int CreatorId { get; set;}
}