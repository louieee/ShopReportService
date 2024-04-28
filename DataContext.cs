using Microsoft.EntityFrameworkCore;
using ReportService.Models;

namespace ReportApp.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public virtual DbSet<User> Users{ get; set; }
    public virtual DbSet<UserView> UserViews { get; set; }

    
}