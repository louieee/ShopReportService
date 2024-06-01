using ReportApp.Data;
using ReportService.Handlers;

namespace ReportService.Helpers

{
    /// <summary>
    /// Searches the request source for the value of specified key
    /// </summary>
    public class HttpFetchValueHelper
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly DataContext _dataContext;
        public HttpFetchValueHelper(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            _httpContext = httpContextAccessor;
            _dataContext = dataContext;
        }
        public string GetValueFromSource(string Key)
        {
            var authHandler = new AuthHandler(_httpContext, _dataContext);
            string? id;
            if (!authHandler.IsValidUser(Key))
            {
                var val = new Microsoft.Extensions.Primitives.StringValues();
                var fromQuery = _httpContext.HttpContext != null && _httpContext.HttpContext.Request.Query.TryGetValue(Key, out val);
                //var fromPath = _HttpContext.HttpContext.Request.Path.Value;
                id = !fromQuery ? null : val.ToString();
                if (id == null)
                    return string.Empty;
            }
            else
            {
                var val = new Microsoft.Extensions.Primitives.StringValues();
                var fromQuery = _httpContext.HttpContext != null && _httpContext.HttpContext.Request.Query.TryGetValue(Key, out val);
                //var fromPath = _HttpContext.HttpContext.Request.Path.Value;
                id = !fromQuery ? null : val.ToString();
                if (id == null)
                    return string.Empty;
            }
            return id;
        }
    }
}
