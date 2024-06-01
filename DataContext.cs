using Microsoft.EntityFrameworkCore;
using ReportApp.Data.Models.communication;
using ReportApp.Data.Models.crm;
using ReportApp.Data.Models.inventory;
using ReportService.Models;

namespace ReportApp.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<User> Users{ get; set; }
    public virtual DbSet<Staff> Staffs { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    
    public virtual DbSet<Chat> Chats { get; set; }
    public virtual DbSet<Group?> Groups { get; set; }
    
    public virtual DbSet<Contact> Contacts { get; set; }
    public virtual DbSet<Lead> Leads { get; set; }
    
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Sale> Sales { get; set; }
    
    public virtual DbSet<CustomerUserReportByYear> CustomerReportByYear { get; set; }
    public virtual DbSet<CustomerUserReportByMonth> CustomerReportByMonth { get; set; }
    public virtual DbSet<CustomerUserReportByDay> CustomerReportByDay { get; set; }
    public virtual DbSet<CustomerUserReportByHour> CustomerReportByHour { get; set; }
    public virtual DbSet<StaffUserReportByYear> StaffReportByYear { get; set; }
    public virtual DbSet<StaffUserReportByMonth> StaffReportByMonth { get; set; }
    public virtual DbSet<StaffUserReportByDay> StaffReportByDay { get; set; }
    public virtual DbSet<StaffUserReportByHour> StaffReportByHour { get; set; }
    
    public virtual DbSet<ChatUsers> ChatUsers { get; set; }
    public virtual DbSet<GroupCreators> GroupCreators { get; set; }
    public virtual DbSet<PrivateChats> PrivateChats { get; set; }
    
    public virtual DbSet<LeadReportByYear> LeadReportByYear { get; set; }
    public virtual DbSet<LeadReportByDay> LeadReportByDay { get; set; }
    public virtual DbSet<LeadReportByMonth> LeadReportByMonth { get; set; }
    public virtual DbSet<LeadReportByHour> LeadReportByHour { get; set; }
    public virtual DbSet<ContactReportByYear> ContactReportByYear { get; set; }
    public virtual DbSet<ContactReportByMonth> ContactReportByMonth { get; set; }
    public virtual DbSet<ContactReportByDay> ContactReportByDay { get; set; }
    public virtual DbSet<ContactReportByHour> ContactReportByHour { get; set; }
    public virtual DbSet<ConvertedLeadsReportByYear> ConvertedLeadsReportByYear { get; set; }
    public virtual DbSet<ConvertedLeadsReportByMonth> ConvertedLeadsReportByMonth { get; set; }
    public virtual DbSet<ConvertedLeadsReportByDay> ConvertedLeadsReportByDay { get; set; }
    public virtual DbSet<ConvertedLeadsReportByHour> ConvertedLeadsReportByHour { get; set; }
    public virtual DbSet<StaffLeadCount> StaffLeadCount { get; set; }
    public virtual DbSet<StaffContactCount> StaffContactCount { get; set; }
    public virtual DbSet<LeadDetail> LeadDetails { get; set; }
    
    public virtual DbSet<StaffDeliveredProduct> StaffDeliveredProducts { get; set; }
    public virtual DbSet<ProductOrders> ProductOrders { get; set; }
    public virtual DbSet<ProductOrderedByAgeView> ProductOrderedByAgeView { get; set; }
    public virtual DbSet<ProductOrderedByGenderView> ProductOrderedByGenderView { get; set; }
    public virtual DbSet<ProductOrderedByLocationView> ProductOrderedByLocationView { get; set; }
    public virtual DbSet<ProductOrderedByQuantityView> ProductOrderedByQuantityView { get; set; }
    public virtual DbSet<ProductSpeedView> ProductSpeedView { get; set; } 
    public virtual DbSet<ProductsSoldPerYear> ProductsSoldPerYear { get; set; }
    public virtual DbSet<ProductsSoldPerMonth> ProductsSoldPerMonth { get; set; }
    public virtual DbSet<ProductsSoldPerDay> ProductsSoldPerDay { get; set; }
    public virtual DbSet<ProductsSoldPerHour> ProductsSoldPerHour { get; set; }
    
    
    
    

    
}