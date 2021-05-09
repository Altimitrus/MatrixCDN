using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MatrixCDN.Engine.Middlewares
{
    public class Accs
    {
        private readonly RequestDelegate _next;
        public Accs(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Access-Control-Request-Headers", out var AccessControl) && AccessControl == "authorization")
            {
                httpContext.Response.StatusCode = 204;
                return Task.CompletedTask;
            }

            if (Startup.accs != null && !Regex.IsMatch(httpContext.Request.Path.Value, "^/(echo|settings|viewed|cron/|$)"))
            {
                if (httpContext.Request.Headers.TryGetValue("Authorization", out var Authorization))
                {
                    byte[] data = Convert.FromBase64String(Authorization.ToString().Replace("Basic ", ""));
                    string[] decodedString = Encoding.UTF8.GetString(data).Split(":");

                    string login = decodedString[0];
                    string passwd = decodedString[1];

                    if (Startup.accs.TryGetValue(login, out string _pass) && _pass == passwd)
                        return _next(httpContext);
                }

                httpContext.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            return _next(httpContext);
        }
    }
}
