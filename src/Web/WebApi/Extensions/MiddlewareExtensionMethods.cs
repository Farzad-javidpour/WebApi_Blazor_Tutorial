using Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Collections.Generic;

namespace WebApi.Extensions
{
    public static class MiddlewareExtensionMethods
    {
        //-----------------------------------------------------------------------------------------------------------------
        public static void UseCommonMiddlewares(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
            app.UseErrorHandler(debugMode: true);
            app.UseResponseWrapper();
        }
        //-----------------------------------------------------------------------------------------------------------------
        public static void UseCustomSwagger(this IApplicationBuilder app, Dictionary<string, string> keyValues)
        {
            app.UseSwagger();
            foreach (var item in keyValues)
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/{item.Key}/swagger.json", item.Value + " - " + item.Key);
                });
            }

        }
        //-----------------------------------------------------------------------------------------------------------------
    }
}
