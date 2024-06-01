using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ReportApp.Data.Models.communication;

public class ChatUsers
{
    [Key]
    [Column("id")]
    public int UserId { get; set; }
    
    [Column("firstname")]
    public string FirstName { get; set; }
    
    [Column("lastname")]
    public string LastName { get; set; }
    
    [Column("chat_count")]
    public int ChatCount { get; set;}
    
    [Column("group_count")]
    public int GroupCount { get; set;}
    
    [Column("is_customer")]
    public bool IsCustomer { get; set;}
    
    [Column("is_staff")]
    public bool IsStaff { get; set; }
}


public class GroupCreators
{
    [Key]
    [Column("id")]
    public int UserId { get; set; }
    
    [Column("firstname")]
    public string FirstName { get; set; }
    
    [Column("lastname")]
    public string LastName { get; set; }
    
    [Column("groups_created")]
    public int GroupCount { get; set;}
    
    [Column("type")]
    public string GroupType { get; set;}
    
    [Column("number_of_participants")]
    public int ParticipantCount { get; set; }
    
    [Column("is_staff")]
    public bool IsStaff { get; set;}
}

public class PrivateChats
{
    [Key]
    [Column("id")]
    public string ChatId { get; set; }
    
    [Column("staff")]
    public string Staff { get; set; }
    
    [Column("staff_user_id")]
    public int StaffUserId { get; set; }
    
    [Column("customer_user_id")]
    public int CustomerUserId { get; set;}
    
    [Column("customer")]
    public string Customer { get; set;}
    
    [Column("date_connected")]
    public DateTime DateConnected { get; set; }
}
