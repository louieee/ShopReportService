using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Data.Models.communication;


[Table("chat")]
public partial class Chat
{
   
    [Key]
    [Column("id")]
    public string Id { get; set; }
    [Column("groupid")]
    public string? GroupId { get; set; }
    [Column("isgroup")]
    public bool IsGroup { get; set; }
    
    [Column("date_connected")]
    public DateTime DateConnected { get; set; }
}


[Table("chat_participants")]
public partial class ChatParticipant
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("chat_id")]
    public string ChatId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

}