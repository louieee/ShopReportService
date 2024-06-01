using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ReportApp.Data.Requests.acccounts;

namespace ReportService.Models;


public class UserReportByYear
{
    [Column("year")]
    public int Year { get; set; }
    
}


public class UserReportByMonth
{
    [Column("month")]
    public MonthEnum Month { get; set; }
    [Column("year")]
    public int Year { get; set; }

    public string MonthStr => Month.ToString();
}


public class UserReportByDay
{
    [Column("date_joined")]
    public DateOnly DateJoined { get; set; }
}

public class UserReportByHour
{
    [Column("date_joined")]
    public DateOnly DateJoined { get; set; }
    [Column("hour_joined")]
    public int HourJoined { get; set; }
}


[Keyless]
public class CustomerUserReportByYear : UserReportByYear
{
    [Column("customer_count")]
    public int CustomerCount { get; set; }
}

[Keyless]
public class CustomerUserReportByMonth:UserReportByMonth
{
    
    [Column("customer_count")]
    public int CustomerCount { get; set; }
}

[Keyless]
public class CustomerUserReportByDay:UserReportByDay
{
    [Column("customer_count")]
    public int CustomerCount { get; set; }
}
[Keyless]
public class CustomerUserReportByHour:UserReportByHour
{
    [Column("customer_count")]
    public int CustomerCount { get; set; }
}

[Keyless]
public class StaffUserReportByYear:UserReportByYear
{
    [Column("staff_count")]
    public int StaffCount { get; set; }
}

[Keyless]
public class StaffUserReportByMonth:UserReportByMonth
{
     [Column("staff_count")]
    public int StaffCount { get; set; }
    
}

[Keyless]
public class StaffUserReportByDay: UserReportByDay
{
    [Column("staff_count")]
    public int StaffCount { get; set; }
}
[Keyless]
public class StaffUserReportByHour: UserReportByHour
{
    [Column("staff_count")]
    public int StaffCount { get; set; }
}