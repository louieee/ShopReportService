using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.communication;
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
    [Route("api/communications")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class CommunicationControllers : ControllerBase
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DataContext _DbContext;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IConfiguration _config;
        private AuthHandler _AuthHandler;
        private HttpFetchValueHelper FetchValueHelper;
        MailHelper _MailHelper;


        public CommunicationControllers(DataContext context, IHttpContextAccessor httpContext, IWebHostEnvironment environment,
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
        /// Retrieves list of users and the number of chats and groups they are in
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/users-chats")]
        // [Authorize]
        public async Task<ActionResult<ChatUsers[]>> UserChat()
        {
            var query = _DbContext.ChatUsers
                .FromSqlRaw("with user_communications as " +
                            "(\nselect u.id, trim(u.firstname) as firstname, trim(u.lastname) as lastname,\n" +
                            " (count(c.id) - count(g.id)) as chat_count,\n " +
                            "count(g.id) as group_count,\n (u.customerid is not null)" +
                            " as is_customer,\n (u.staffid is not null) as is_staff\n " +
                            " \n\nfrom users u join chat_participants cp on u.id = cp.user_id join \n" +
                            "chat c on cp.chat_id = c.id left join groups g on c.groupid = g.id group by u.id)\n" +
                            "\nselect * from user_communications;");
            return Ok(query);
        }
        
        
        /// <summary>
        /// Retrieves list of users and the number of groups they created
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/group-creators")]
        // [Authorize]
        public async Task<ActionResult<GroupCreators[]>> GroupCreatorsReport()
        {
            var query = _DbContext.GroupCreators
                .FromSqlRaw("select u.id, trim(u.firstname) as firstname, trim(u.lastname) as lastname,\n" +
                            "count(g.id) as groups_created,\ng.type,\n" +
                            "count(cp.*) as number_of_participants,\n(u.customerid is not null)" +
                            " as is_customer,\n (u.staffid is not null) as is_staff\n" +
                            "from groups g join users u on g.creatorid = u.id join chat c on g.id = c.groupid" +
                            "\njoin chat_participants cp on c.id = cp.chat_id group by g.type, u.id;");
            return Ok(query);
        }
        
        
        /// <summary>
        /// Retrieves list of private chats between staffs and customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reports/private-chats")]
        // [Authorize]
        public async Task<ActionResult<PrivateChats[]>> PrivateChatReport()
        {
            var query = _DbContext.PrivateChats
                .FromSqlRaw("select trim(c.id) as id, concat(trim(us.firstname), ' ', trim(us.lastname)) as staff," +
                            " us.id as staff_user_id , (select concat(trim(firstname), ' ', trim(lastname)) " +
                            "from users u join chat_participants \n    cp on u.id = cp.user_id and cp.chat_id = c.id" +
                            " and u.staffid is null limit 1) as customer,\n (select u.id from users u" +
                            " join chat_participants\n cp on u.id = cp.user_id and cp.chat_id = c.id and u.staffid is " +
                            "null limit 1) as customer_user_id, c.date_connected\n    \nfrom chat c \n         join chat_participants cps" +
                            " on c.id = cps.chat_id join users us on cps.user_id = us.id and us.staffid is not null " +
                            "\nwhere c.isgroup is false;");
            return Ok(query);
        }


    }
}


