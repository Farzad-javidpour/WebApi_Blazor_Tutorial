using Infrastructure.Common.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    public class ErrorHandlerMiddleware
    {
        #region Properties
        private readonly RequestDelegate _next;
        public bool DebugMode { get; }
        #endregion

        #region Constructor
        public ErrorHandlerMiddleware(bool debugMode, RequestDelegate next)
        {
            DebugMode = debugMode;
            _next = next;
        }
        #endregion

        #region Methods
        public async Task Invoke(HttpContext context)
        {
            Stream responseBody = context.Response.Body;

            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                //----------------------------------------------------
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case BadHttpRequestException:
                    case ArgumentNullException:
                    case ArgumentOutOfRangeException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case AccessViolationException:

                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;

                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                //----------------------------------------------------
                var responseModel = await ApiResponse.FailAsync();
                responseModel.Path = context.Request.Path.ToString();
                responseModel.Method = context.Request.Method.ToString();
                responseModel.ExceptionErrors = new List<string>();
                responseModel.ExceptionErrors.Add(StatusCodeMessage(response.StatusCode));
                if (DebugMode)
                {
                    responseModel.ExceptionErrors.Add(error.ToString());
                }
                //----------------------------------------------------

                var result = JsonSerializer.Serialize(responseModel);

                context.Response.Body = responseBody;
                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                await context.Response.Body.WriteAsync(resultBytes, 0, resultBytes.Length);

            }
        }

        #region Helper Methods
        private string StatusCodeMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad request.";
                case 401:
                    return "Unauthorized access.";
                case 402:
                    return "Payment required.";
                case 403:
                    return "Forbidden access.";
                case 404:
                    return "Resource not found.";
                case 405:
                    return "Method not allowed.";
                case 406:
                    return "Not acceptable.";
                case 407:
                    return "Proxy authentication required.";
                case 408:
                    return "Request timeout.";
                case 409:
                    return "Conflict";
                case 410:
                    return "Resource is gone.";
                case 411:
                    return "Length is required.";
                case 500:
                    return "Internal server error.";
                case 501:
                    return "Not implemented.";
                case 502:
                    return "Bad gateway.";
                case 503:
                    return "Service unavailable.";
                case 504:
                    return "Gateway timeout.";
                case 505:
                    return "HTTP version not supported.";
            }
            return "";
        }
        #endregion
        #endregion

    }

    #region ExtensionMethod
    public static class ErrorHandlerExtension
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app, bool debugMode)
        {
            return app.UseMiddleware<ErrorHandlerMiddleware>(debugMode);
        }
    }
    #endregion
}