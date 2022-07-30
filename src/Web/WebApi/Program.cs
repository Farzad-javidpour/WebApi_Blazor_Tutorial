using Infrastructure.Middleware;
using WebApi.Extensions;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi.Application.Interfaces;
using WebApi.Infrastructure;
using WebApi.Infrastructure.Contexts;
using WebApi.Infrastructure.Repositories;

try
{
    #region Serilog Configuration
    Log.Information("Starting web host");
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile("appsettings.json");

    Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .CreateLogger();

    builder.Host.UseSerilog((context, services, conf) => conf
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console());
    //.WriteTo.Seq("http://localhost:5342"));
    #endregion

    //---------------------------------------------------------------------------

    #region Service Registration
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCustomAutoMapper(Assembly.Load("WebApi.Application"));
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddCustomSwagger(new Dictionary<string, string> { ["v1"] = "WebApi" });
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnectionString"));
    });
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IProductRepository, ProductSqlRepository>();
    #endregion

    var app = builder.Build();

    #region Pipeline

    app.UseCustomSwagger(new Dictionary<string, string> { ["v1"] = "WebApi" });
    app.UseCommonMiddlewares();
    //app.UseHsts();
    //app.UseHttpsRedirection();
    //app.UseStaticFiles();
    //app.UseRouting();
    //app.UseCors(); 
    //app.UseAuthentication();
    //app.UseAuthorization();
    app.MapControllers();

    #endregion

    //---------------------------------------------------------------------------

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

