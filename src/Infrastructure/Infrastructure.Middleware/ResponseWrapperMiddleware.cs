using Infrastructure.Common.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream responseBody = context.Response.Body;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;
                //----------------------------------------------------------------
                await _next(context);
                //----------------------------------------------------------------
                memoryStream.Position = 0;
                string responseString = new StreamReader(memoryStream).ReadToEnd();
                string wrappedResponse = this.Wrap(responseString, context);
                byte[] responseBytes = Encoding.UTF8.GetBytes(wrappedResponse);

                context.Response.Headers["Content-type"] = "application/json";
                context.Response.Body = responseBody;
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        #region Helper Methods
        private string Wrap(string originalBody, HttpContext context)
        {
            dynamic response;

            if (this.IsJsonString<ExpandoObject>(originalBody))
                response = JsonSerializer.Deserialize<ExpandoObject>(originalBody);
            else
                response = originalBody;

            response.path = context.Request.Path.ToString();
            response.method = context.Request.Method.ToString();

            object wrapper = response;
            if (this.IsSuccessResponse(context.Response.StatusCode) == false &&
                response.title.ToString().Equals("One or more validation errors occurred."))
            {
                var error = response.errors.ToString().Replace("{", "").Replace("}", "").Replace("\"", "");
                var errorList = error.Split(',');
                wrapper = ApiResponse.Fail(new List<string>(errorList));
            }
            else
            {
                wrapper = response;
            }

            string newBody = JsonSerializer.Serialize(wrapper);

            return newBody;
        }

        private bool IsJsonString<T>(string text)
        {
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || (text.StartsWith("[") && text.EndsWith("]")))
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<T>(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsSuccessResponse(int statusCode)
        {
            return (statusCode >= 200 && statusCode < 299);
        }

        #endregion
    }

    #region ExtensionMethod
    public static class ResponseWrapperExtension
    {
        public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseWrapperMiddleware>();
        }
    }
    #endregion
}