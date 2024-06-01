using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ReportService.Helpers;

namespace ReportApp.Data.Models.crm;

[Keyless]
public class CRMReportByYear
{
    [Column("year")]
    public int Year { get; set; }
    
}

[Keyless]
public class CRMReportByMonth
{
    [Column("month")]
    public VarHelper.MonthEnum Month { get; set; }
    [Column("year")]
    public int Year { get; set; }

    public string MonthStr => Month.ToString();
}

[Keyless]
public class CRMReportByDay
{
    [Column("date_created")]
    public DateOnly DateCreated { get; set; }
}
[Keyless]
public class CRMReportByHour
{
    [Column("date_created")]
    public DateOnly DateCreated { get; set; }
    [Column("hour_created")]
    public int Hourcreated { get; set; }
}


public class LeadReportByYear : CRMReportByYear
{
    [Column("lead_count")]
    public int LeadCount { get; set; }
    
}

public class LeadReportByMonth : CRMReportByMonth
{
    [Column("lead_count")]
    public int LeadCount { get; set; }
}

public class LeadReportByDay : CRMReportByDay
{
    [Column("lead_count")]
    public int LeadCount { get; set; }
}

public class LeadReportByHour : CRMReportByHour
{
    [Column("lead_count")]
    public int LeadCount { get; set; }
}

public class ContactReportByYear : CRMReportByYear
{
    [Column("contact_count")]
    public int ContactCount { get; set; }
    
}

public class ContactReportByMonth : CRMReportByMonth
{
    [Column("contact_count")]
    public int ContactCount { get; set; }
}

public class ContactReportByDay : CRMReportByDay
{
    [Column("contact_count")]
    public int ContactCount { get; set; }
}

public class ContactReportByHour : CRMReportByHour
{
    [Column("contact_count")]
    public int ContactCount { get; set; }
}

[Keyless]
public class ConvertedLeadsReportByYear
{
    [Column("year")]
    public int Year { get; set; }
    [Column("lead_count")]
    public int LeadCount { get; set; }
    
}

[Keyless]
public class ConvertedLeadsReportByMonth
{
    [Column("month")]
    public VarHelper.MonthEnum Month { get; set; }
    [Column("lead_count")]
    public int LeadCount { get; set; }
    [Column("year")]
    public int Year { get; set; }

    public string MonthStr => Month.ToString();
}

[Keyless]
public class ConvertedLeadsReportByDay
{
    [Column("date_converted")]
    public DateOnly DateConverted { get; set; }
    [Column("lead_count")]
    public int LeadCount { get; set; }
}

[Keyless]
public class ConvertedLeadsReportByHour
{
    [Column("date_converted")]
    public DateOnly DateConverted { get; set; }
    [Column("hour_converted")]
    public int HourConverted { get; set; }
    [Column("lead_count")]
    public int LeadCount { get; set; }
    
}

[Keyless]
public class StaffCRMCount
{
    [Column("id")]
    public int StaffUserId { get; set; }
    [Column("staff")]
    public string Staff { get; set; }
    [Column("staffid")]
    public int StaffId { get; set; }
    [Column("gender")]
    public string Gender { get; set; }
}

[Keyless]
public class StaffLeadCount: StaffCRMCount
{
    [Column("leadcount")]
    public int LeadCount { get; set; }
}

public class StaffContactCount: StaffCRMCount
{
    [Column("contact_count")]
    public int ContactCount { get; set; }
}


public class LeadDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("title")]
    public string Title { get; set; }
    
    [Column("company")]
    public string Company { get; set; }
    
    [Column("lead_contact")]
    public int LeadContact { get; set; }
    
    [Column("lead_contact_id")]
    public string LeadContactId { get; set; }
    
    [Column("staff_id")]
    public int StaffId { get; set; }
    
    [Column("staff")]
    public string Staff { get; set; }
    
    [Column("staff_gender")]
    public string StaffGender { get; set; }
    
    [Column("lead_age")]
    public int LeadAge { get; set; }
}
