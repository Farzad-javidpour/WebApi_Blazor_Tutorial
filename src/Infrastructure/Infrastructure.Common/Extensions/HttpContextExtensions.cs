using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserIdentifier(this HttpContext httpContext)
        {
            var userIdentifier = httpContext.User.Claims.FirstOrDefault(x => x.Type.Contains("client_id"))?.Value;
            return userIdentifier;
        }
        public static string GetIpAddress(this HttpContext httpContext)
        {
            string ip = httpContext.Connection.RemoteIpAddress.ToString();
            
            return ip;
        }
    }
}
