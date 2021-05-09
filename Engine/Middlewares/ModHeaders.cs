using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MatrixCDN.Engine.Middlewares
{
    public class ModHeaders
    {
        private readonly RequestDelegate _next;
        public ModHeaders(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Accept, Content-Type");
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            //System.Console.WriteLine("\n\n" + httpContext.Request.Path.Value + httpContext.Request.QueryString.Value);

            return _next(httpContext);
        }
    }
}
