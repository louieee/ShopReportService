using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.crm;
using ReportApp.Data.Requests.acccounts;
using ReportService.Handlers;
using ReportService.Helpers;
using ReportService.Models;
using ReportService.Responses;

/*
 * get logged in user's profile
 *
 * 
 */

namespace ReportService.Controllers
{
    /// <summary>
    /// User Management APIs
    /// </summary>
    [Route("api/crm"),ApiController,
     EnableCors("AllowAllHeaders")]
    public class CRMControllers : ControllerBase
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DataContext _DbContext;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IConfiguration _config;
        private AuthHandler _AuthHandler;
        private HttpFetchValueHelper FetchValueHelper;
        MailHelper _MailHelper;


        public CRMControllers(DataContext context, IHttpContextAccessor httpContext, IWebHostEnvironment environment,
            IConfiguration config)
        {
            _config = config;
            _WebHostEnvironment = environment;
            _DbContext = context;
            _HttpContext = httpContext;
            _AuthHandler = new AuthHandler(_HttpContext, _DbContext);
            FetchValueHelper = new HttpFetchValueHelper(_HttpContext, _DbContext);
            _MailHelper = new MailHelper(config);
        }

        /// <summary>
        /// Retrieves lead counts at time durations
        /// </summary>
        /// <param name="time">Choices are time daily, monthly, yearly and hourly</param>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/leads-count")]
        // [Authorize]
        public async Task<ActionResult> LeadCountReport([FromQuery] string time)
        {
            if (time != null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.LeadReportByYear
                    .FromSqlRaw("select  extract(year from l.datecreated) as year,\n" +
                                "count(*) as lead_count from lead l group by year;\n");
                return Ok(val);
            }
            if (time != null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.LeadReportByMonth
                    .FromSqlRaw("select  extract(month from l.datecreated) as month,\n" +
                                "extract(year from l.datecreated) as year,\n" +
                                "count(*) as lead_count from lead l where extract(year from l.datecreated) = " +
                                "extract(year from current_date)\n group by month, year;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.LeadReportByDay
                    .FromSqlRaw("select  date(l.datecreated) as date_created,\n" +
                                "count(*) as lead_count from lead l where extract(year from l.datecreated) = " +
                                "extract(year from current_date) and\nextract(month from l.datecreated) = " +
                                "extract(month from current_date)\n \ngroup by date_created;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.LeadReportByHour
                    .FromSqlRaw("select  date(l.datecreated) as date_created,\n" +
                                "extract(hour from l.datecreated) as hour_created,\n" +
                                "count(*) as lead_count from lead l  where l.datecreated = current_date\n" +
                                "group by date_created, hour_created;");
                return Ok(val);
            }

            return Ok();
        }
        
        /// <summary>
        /// Retrieves contacts counts at time durations
        /// </summary>
        /// <param name="time">Choices are time daily, monthly, yearly and hourly</param>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/contacts-count")]
        // [Authorize]
        public async Task<ActionResult> ContactCountReport([FromQuery] string time)
        {
            if (time != null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ContactReportByYear
                    .FromSqlRaw("select  extract(year from c.datecreated) as year,\n" +
                                "count(*) as contact_count from contact c group by year;\n");
                return Ok(val);
            }
            if (time != null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ContactReportByMonth
                    .FromSqlRaw("select  extract(month from c.datecreated) as month,\n" +
                                "extract(year from c.datecreated) as year,\n" +
                                "count(*) as contact_count from contact c where extract(year from c.datecreated) = " +
                                "extract(year from current_date)\n group by month, year;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ContactReportByDay
                    .FromSqlRaw("select  date(c.datecreated) as date_created,\n" +
                                "count(*) as contact_count from contact c where extract(year from c.datecreated) = " +
                                "extract(year from current_date) and\nextract(month from c.datecreated) = " +
                                "extract(month from current_date)\n \ngroup by date_created;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ContactReportByHour
                    .FromSqlRaw("select  date(c.datecreated) as date_created,\n" +
                                "extract(hour from c.datecreated) as hour_created,\n" +
                                "count(*) as contact_count from contact c  where c.datecreated = current_date\n" +
                                "group by date_created, hour_created;");
                return Ok(val);
            }

            return Ok();
        }
        
        /// <summary>
        /// Retrieves converted leads counts at time durations
        /// </summary>
        /// <param name="time">Choices are time daily, monthly, yearly and hourly</param>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/converted-leads-count")]
        // [Authorize]
        public async Task<ActionResult> ConvertedLeadsCountReport([FromQuery] string time)
        {
            if (time != null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ConvertedLeadsReportByYear
                    .FromSqlRaw("select  extract(year from l.conversiondate) as year,\n" +
                                "count(*) as lead_count from lead  l where l.isdeal group by year;");
                return Ok(val);
            }
            if (time != null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ConvertedLeadsReportByMonth
                    .FromSqlRaw("with month_year as (select  distinct  extract(month from conversiondate) as month,\n" +
                                "extract(year from conversiondate) as year\nfrom lead where conversiondate is not null " +
                                "and lead.isdeal)\nselect year, month, (select count(id) from lead l where extract(year " +
                                "from l.conversiondate ) = year \n   and month = extract(month from l.conversiondate ) " +
                                "and l.isdeal and l.conversiondate is not null) as lead_count  \nfrom month_year where " +
                                "year = extract(year from current_date)");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ConvertedLeadsReportByDay
                    .FromSqlRaw("with lead_date as (select  distinct  date(conversiondate) as date_converted\n" +
                                "from lead where conversiondate is not null and lead.isdeal and\n" +
                                "extract(month from conversiondate) = extract(month from current_date) and\n" +
                                "extract(year from conversiondate) = extract(year from current_date))\nselect " +
                                "date_converted, (select count(id) from lead l where date(l.conversiondate ) " +
                                "= date_converted\nand l.isdeal and l.conversiondate is not null) as lead_count\n" +
                                "from lead_date;\n");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.ConvertedLeadsReportByHour
                    .FromSqlRaw("with lead_date_hr as (select  distinct  date(conversiondate) as date_converted,\n" +
                                "(extract(hour from conversiondate)) as hour_converted\nfrom lead where conversiondate" +
                                " is not null and lead.isdeal\nand conversiondate = current_date)\nselect date_converted, " +
                                "hour_converted, (select count(id) from lead l where date(l.conversiondate ) = date_converted\n" +
                                "and extract(hour from l.conversiondate) = hour_converted and\n" +
                                "l.isdeal and l.conversiondate is not null) as lead_count\nfrom lead_date_hr;");
                return Ok(val);
            }

            return Ok();
        }
        
        /// <summary>
        /// Retrieves report of leads by staffs 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/staff-leads-count")]
        // [Authorize]
        public async Task<ActionResult<StaffLeadCount[]>> StaffLeadsCountReport()
        {
            var query = _DbContext.StaffLeadCount
                .FromSqlRaw("select u.id, concat(trim(u.firstname),' ', trim(u.lastname)) as staff, "+
                            "u.staffid, trim(u.gender) as gender, count(l.id) as leadCount from users u left join lead l on u.id = " +
                            "l.ownerid\nwhere u.staffid is not null group by u.id;");
            return Ok(query);
        }
        
        /// <summary>
        /// Retrieves report of contacts by staffs 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/staff-contacts-count")]
        // [Authorize]
        public async Task<ActionResult<StaffContactCount[]>> StaffContactsCountReport()
        {
            var query = _DbContext.StaffContactCount
                .FromSqlRaw("select u.id, concat(trim(u.firstname),' ', trim(u.lastname)) as staff, u.staffid, trim(u.gender) as gender, " +
                            "count(c.id) as contact_count from users u left join contact c on u.id = c.ownerid\n" +
                            "where u.staffid is not null group by u.id;");
            return Ok(query);
        }


        
        
    }
}


