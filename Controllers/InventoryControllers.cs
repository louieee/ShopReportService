using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.inventory;
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
    [Route("api/inventories/reports")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class InventoryControllers : ControllerBase
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DataContext _DbContext;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IConfiguration _config;
        private AuthHandler _AuthHandler;
        private HttpFetchValueHelper FetchValueHelper;
        MailHelper _MailHelper;


        public InventoryControllers(DataContext context, IHttpContextAccessor httpContext, IWebHostEnvironment environment,
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
        /// Retrieves number of products sold over time duration
        /// </summary>
        /// <param name="time">Options are yearly, monthly, daily, hourly</param>
        /// <param name="productId">Filter by product Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("products-sold")]
        // [Authorize]
        public async Task<ActionResult<UserProfileResponse>> ProductsSalesReport(
            [FromQuery] string? time, [FromQuery] int? productId)
        {
            var productQuery = productId == null ? "" : "and p.id = {0} ";
            var queryParam = new List<Object> { };
            if (productId != null)
            {
                queryParam.Add(productId);
            }
            if (time!= null && time.Equals(TimeFilter.Yearly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var query = _DbContext.ProductsSoldPerYear
                    .FromSqlRaw("select extract(year from s.datepaid) as year ,\n" +
                                "count(*) as products_sold\nfrom product p join orders o on p.id = o.productid join " +
                                "sale s on o.saleid = s.id where s.paid = true\n " +
                                $"{productQuery}"+
                                "group by year;", queryParam.ToArray());
                return Ok(query);
            }
            
            if (time!= null && time.Equals(TimeFilter.Monthly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var query = _DbContext.ProductsSoldPerMonth
                    .FromSqlRaw("select extract(year from s.datepaid) as year,  extract(month from s.datepaid) as month,\n" +
                                "count(*) as products_sold\nfrom product p join orders o on p.id = o.productid join sale s " +
                                "on o.saleid = s.id where s.paid = true\nand extract(year from s.datepaid) = extract(year" +
                                " from current_date)\n" + $"{productQuery}"+
                                "group by year, month;", queryParam.ToArray());
                return Ok(query);
            }
            if (time!= null && time.Equals(TimeFilter.Daily.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var query = _DbContext.ProductsSoldPerDay
                    .FromSqlRaw("select s.datepaid as date_sold,\n" +
                                "count(*) as products_sold\nfrom product p join orders o on p.id = " +
                                "o.productid join sale s on o.saleid = s.id where s.paid = true\nand extract(year " +
                                "from s.datepaid) = extract(year from current_date)\nand extract(month from s.datepaid)" +
                                " = extract(month from current_date)\n" + $"{productQuery}"+
                                "group by s.datepaid", queryParam.ToArray());
                return Ok(query);
            }
            if (time!= null && time.Equals(TimeFilter.Hourly.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                var query = _DbContext.ProductsSoldPerHour
                    .FromSqlRaw("select s.datepaid as date_sold, extract(hour from s.datepaid) as hour_sold,\n" +
                                "count(*) as products_sold\nfrom product p join orders o on p.id = o.productid join " +
                                "sale s on o.saleid = s.id where s.paid = true\n and s.datepaid = current_date\n" +
                                $"{productQuery}" +
                                "group by s.datepaid;", queryParam.ToArray());
                return Ok(query);
            }
            
            return Ok();
        }
        
        /// <summary>
        /// Retrieves staffs and the report of products delivered over a period of time
        /// </summary>
        /// <param name="startDate">Start date query; format is YYYY-MM-DD</param>
        /// <param name="endDate">End date query; format is YYYY-MM-DD</param>
        /// <returns></returns>
        [HttpGet]
        [Route("products-delivery")]
        // [Authorize]
        public async Task<ActionResult<StaffDeliveredProduct[]>> ProductDeliveryReport(
            [FromQuery, DataType(DataType.Date)] string? startDate, [FromQuery, DataType(DataType.Date)] string? endDate)
        {
            var dateQuery = "";
            Console.WriteLine($"start date: {startDate} ");
            var queryParams = new List<object> { };
            if (startDate != null && endDate == null)
            {
                dateQuery = " where datedelivered >= {0} ";
                queryParams.Add(DateOnly.Parse(startDate));
            }else if (startDate == null && endDate != null)
            {
                dateQuery = " where datedelivered <= {0} ";
                queryParams.Add(DateOnly.Parse(endDate));
            }
            else if (startDate != null && endDate != null)
            {
                dateQuery = " where datedelivered between {0} and {1} ";
                queryParams.Add(DateOnly.Parse(startDate));
                queryParams.Add(DateOnly.Parse(endDate));
            }

            var query = _DbContext.StaffDeliveredProducts
                .FromSqlRaw("select (concat(trim(u.firstname), ' ', trim(u.lastname))) as staff,\n" +
                            "count(p.id) as products_delivered\nfrom product p join orders o on p.id = o.productid " +
                            "and o.delivered = true join sale s on o.saleid = s.id and s.paid = true\n" +
                            "join users u on o.staffid = u.staffid\n" +
                            $"{dateQuery}" +
                            "\ngroup by staff;", queryParams.ToArray());
            
            return Ok(query);
        }

        /// <summary>
        /// Retrieves data of product order demography report
        /// </summary>
        /// <param name="sortBy">Options are productsOrderedByAge, productsOrderedByGender, productsOrderedByLocation,
        /// productsOrderedByQuantity</param>
        /// <param name="productId">Filter Data by product</param>
        /// <param name="delivered">Filter table data by delivered</param>
        /// <param name="paid">Filter table data by paid</param>
        /// <param name="gender"></param>
        /// <param name="location"></param>
        /// <param name="startAge"></param>
        /// <param name="endAge"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders-demography")]
        // [Authorize]
        public async Task<ActionResult<ProductOrders[]>> ProductOrderDemographyReport(
            [FromQuery] string? sortBy, [FromQuery] int? productId, [FromQuery] bool? delivered, 
            [FromQuery] bool? paid, [FromQuery] string? location, [FromQuery] string? gender,
            [FromQuery] int? startAge, [FromQuery] int? endAge)
        
        {
            var whereQuery = "";
            var queryParams = new List<object>();
            if (productId != null)
            {
                whereQuery = "and product_id = {0} ";
                queryParams.Add(productId);
            }
            if (sortBy == "productsOrderedByAge")
            {
                var query = _DbContext.ProductOrderedByAgeView
                    .FromSqlRaw("select age, count(id) as products_ordered from orders_view" +
                                " where paid = true "+whereQuery+
                                "group by age;", queryParams.ToArray());
                return Ok(query);
            }
            if (sortBy == "productsOrderedByGender")
            {
                var query = _DbContext.ProductOrderedByGenderView
                    .FromSqlRaw("select gender, count(id) as products_ordered from orders_view " +
                                " where paid = true "+whereQuery+
                                "group by gender;", queryParams.ToArray())
                    .AsNoTracking();
                return Ok(query);
            }

            if (sortBy == "productsOrderedByLocation")
            {
                var query = _DbContext.ProductOrderedByLocationView
                    .FromSqlRaw("select location, count(id) as products_ordered from orders_view" +
                                " where paid = true "+ whereQuery+
                                " group by location;", queryParams.ToArray())
                    .AsNoTracking();
                return Ok(query);
            }

            if (sortBy == "productsOrderedByQuantity")
            {
                queryParams = [];
                whereQuery = "";
                if (location != null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"location  ilike {"+ queryParams.Count + "} ";
                    queryParams.Add($"{location}%");
                }

                if (gender != null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"gender ilike {"+ queryParams.Count + "} ";
                    queryParams.Add($"{gender}%");
                }

                if (productId != null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"product_id =  {"+ queryParams.Count + "} ";
                    queryParams.Add(productId);
                }

                if (startAge != null && endAge == null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"age >= {"+ queryParams.Count + "} ";
                    queryParams.Add(startAge);
                }
                else if (startAge == null && endAge != null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"age <= {"+ queryParams.Count + "} ";
                    queryParams.Add(endAge);
                }
                else if (startAge != null && endAge != null)
                {
                    whereQuery += ModifyQuery(whereQuery)+"age between {"+ queryParams.Count + "} and {"+queryParams.Count+1+"} ";
                    queryParams.Add(startAge);
                    queryParams.Add(endAge);
                }
                var query = _DbContext.ProductOrderedByQuantityView
                    .FromSqlRaw("select orders_view.product,  count(*) as  products_ordered, sum(quantity) as " +
                                "total_quantity  from orders_view " +
                                whereQuery+
                                "group by product ;", queryParams.ToArray())
                    .AsNoTracking();
                return Ok(query);
            }

            string ModifyQuery(string x) => x.Contains("where") ? " and " : " where ";
            return Ok();
        }


        /// <summary>
        /// Retrieves data of products sales report
        /// </summary>
        /// <param name="productId">Filter Data by product</param>
        /// <returns></returns>
        [HttpGet]
        [Route("product-sales")]
        // [Authorize]
        public async Task<ActionResult<ProductSpeedView[]>> ProductSalesReport(
            [FromQuery] int? productId)
        {
            var q = _DbContext.ProductSpeedView.AsNoTracking();
            if (productId != null)
            {
                q = q.Where(x => x.ProductId == productId);
            }
            return Ok(q.AsSingleQuery());
        }

        ///<summary>
        /// Retrieves data of product order report
        /// </summary>
        /// <param name="sortBy">Options are productsOrderedByAge, productsOrderedByGender, productsOrderedByLocation,
        /// productsOrderedByQuantity</param>
        /// <param name="productId">Filter Data by product</param>
        /// <param name="delivered">Filter table data by delivered</param>
        /// <param name="paid">Filter table data by paid</param>
        /// <param name="gender"></param>
        /// <param name="location"></param>
        /// <param name="startAge"></param>
        /// <param name="endAge"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("orders")]
        // [Authorize]
        public async Task<ActionResult<ProductOrders[]>> ProductOrderReport(
            [FromQuery] string? sortBy, [FromQuery] int? productId, [FromQuery] bool? delivered, 
            [FromQuery] bool? paid, [FromQuery] string? location, [FromQuery] string? gender,
            [FromQuery] int? startAge, [FromQuery] int? endAge)
        {
            var q = _DbContext.ProductOrders.AsNoTracking();
            if (productId != null)
            {
                q = q.Where(x => x.ProductId == productId);
            }

            if (delivered != null)
            {
                q = q.Where(x => x.Delivered == delivered);
            }

            if (paid != null)
            {
                q = q.Where(x => x.Paid == paid);
            }

            if (startAge != null && endAge == null)
            {
                q = q.Where(x => x.Age >= startAge);
            }else if (startAge == null && endAge != null)
            {
                q = q.Where(x => x.Age <= endAge);
            }else if (startAge != null && endAge != null)
            {
                q = q.Where(x => startAge >= x.Age && x.Age <= endAge);
            }

            if (location != null)
            {
                q = q.Where(x => x.Location.ToLower() == location.ToLower());
            }

            if (gender != null)
            {
                q = q.Where(x => x.Gender.ToLower() == gender.ToLower());
            }
            
    
            return Ok(new {Count=q.Count(), Results=q.AsSingleQuery()});
        }

        


    }
}


