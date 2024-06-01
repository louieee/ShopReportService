using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.communication;
using ReportApp.Data.Requests.acccounts;
using ReportApp.Data.Services;
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
    class PayloadMessage
    {
        public PayloadMessage(string message)
        {
            this.message = message;
        }

        public string message { get; set; }
    }
    /// <summary>
    /// User Management APIs
    /// </summary>
    [Route("api/accounts")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class AccountControllers : ControllerBase
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DataContext _DbContext;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IConfiguration _config;
        private AuthHandler _AuthHandler;
        private HttpFetchValueHelper FetchValueHelper;
        MailHelper _MailHelper;
        private RabbitMQService _rabbitMqService;


        public AccountControllers(DataContext context, IHttpContextAccessor httpContext, IWebHostEnvironment environment,
            IConfiguration config, RabbitMQService rabbitMqService)
        {
            _config = config;
            _WebHostEnvironment = environment;
            _DbContext = context;
            _HttpContext = httpContext;
            _AuthHandler = new AuthHandler(_HttpContext, _DbContext);
            FetchValueHelper = new HttpFetchValueHelper(_HttpContext, _DbContext);
            _MailHelper = new MailHelper(config);
            _rabbitMqService = rabbitMqService;
        }

        /// <summary>
        /// Retrieves complete user profile information
        /// </summary>
        /// <remarks>Bearer authentication is suspended for this endpoint for now to enable you test if registrations are successful. Bearer Authentication will be added before GoLive</remarks>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("loggedInUser")]
        // [Authorize]
        public async Task<ActionResult<UserProfileResponse>> GetLoggedInUserProfile()
        {
            var response = new UserProfileResponse();
            var responseData = new UserProfileData();
            //
            // var user = await _AuthHandler.GetLoggedInUser();
            // responseData.Email = user.Email;
            // responseData.FirstName = user.FirstName;
            // responseData.LastName = user.LastName;
            // responseData.Id = user.Id;
            // responseData.CustomerId = user.CustomerId;
            // responseData.StaffId = user.StaffId;
            var g = JsonConvert.SerializeObject(new PayloadMessage("hello bros"));
            _rabbitMqService.Publish(body:Encoding.UTF8
                .GetBytes(g), exchange: "business", queue:"task");
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "User profile retrieved successfully";
            response.Data = responseData;
            return Ok(response);
        }
        
        
        /// <summary>
        /// Retrieves report of customers by date joined
        /// </summary>
        /// <param name="time">options are yearly, monthly, hourly, daily</param>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/customers/by-date-joined")]
        // [Authorize]
        public async Task<ActionResult> CustomersReportByDateJoined(
            [FromQuery] string? time
                )
        {
            if (time != null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.CustomerReportByYear
                    .FromSqlRaw("select  extract(year from c.DateJoined) as year,count(*) as " +
                        "customer_count from customer c join users u on c.id = " +
                        "u.customerid group by year;");
                return Ok(val);
            }
            if (time != null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.CustomerReportByMonth
                    .FromSqlRaw("" + "select  extract(month from c.DateJoined) as month,  \n        " +
                                "extract(year from c.DateJoined) as year,\n        " +
                                "count(*) as customer_count from customer " +
                                "c join users u on c.id = u.customerid " +
                                "where extract(year from c.DateJoined) = extract(year from current_date) " +
                                "group by month, year;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.CustomerReportByDay
                    .FromSqlRaw("select  date(c.datejoined) as date_joined,\n    " +
                                "count(*) as customer_count from customer c join users u on c.id = u.customerid " +
                                "where extract(year from c.DateJoined) = extract(year from current_date) " +
                                "and extract(month from c.DateJoined) = extract(month from current_date) "+
                                "group by date_joined;\n");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.CustomerReportByHour
                    .FromSqlRaw("select  date(c.datejoined) as date_joined,\n extract(hour from c.datejoined) as hour_joined,\n " +
                                "   count(*) as customer_count from customer c join users u on c.id = u.customerid" +
                                "where c.datejoined = current_date "+
                                "                      \n  group by date_joined, hour_joined\n");
                return Ok(val);
            }

            return Ok();

        }
        
         /// <summary>
        /// Retrieves report of staffs by date joined
        /// </summary>
        /// <param name="time">options are yearly, monthly, hourly, daily</param>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/staff/by-date-joined")]
        // [Authorize]
        public async Task<ActionResult> StaffReportByDateJoined(
            [FromQuery] string? time
                )
        {
            if (time != null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.StaffReportByYear
                    .FromSqlRaw("select  extract(year from c.DateJoined) as year, " +
                                "count(*) as staff_count from staff c join users u on c.id = u.staffid group by year;");
                return Ok(val);
            }
            if (time != null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.StaffReportByMonth
                    .FromSqlRaw("select  extract(month from c.DateJoined) as month," +
                                "extract(year from c.DateJoined) as year, " +
                                "count(*) as staff_count from staff c join users u on c.id = u.staffid " +
                                "where extract(year from c.DateJoined) = extract(year from current_date) " +
                                "group by month, year;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.StaffReportByDay
                    .FromSqlRaw("select  date(c.datejoined) as date_joined, " +
                                "count(*) as staff_count from staff c join users u on c.id = u.staffid " +
                                "where extract(year from c.DateJoined) = extract(year from current_date) and " +
                                "extract(month from c.DateJoined) = extract(month from current_date) " +
                                "group by date_joined;");
                return Ok(val);
            }

            if (time != null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var val = _DbContext.StaffReportByHour
                    .FromSqlRaw("select  date(c.datejoined) as date_joined," +
                                "extract(hour from c.datejoined) as hour_joined," +
                                "count(*) as staff_count from staff c join users u on c.id = u.staffid" +
                                "where c.datejoined >= current_date group by date_joined, hour_joined;");
                return Ok(val);
            }

            return Ok();

        }
    }
}


