using Core.Application.Interfaces;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace WebApi.Infrastructure.Contexts
{
    public class ApplicationDbContext : CustomizedDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options, httpContextAccessor)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //________________________________________________________________________________
            #region Model Configuration

            //Automatically Add all entities to dbContext
            modelBuilder.RegisterAllEntities(Assembly.Load("WebApi.Core"));
            //Automatically Add all entity Configurations to dbContext
            modelBuilder.RegisterEntityTypeConfiguration(typeof(ApplicationDbContext).Assembly);
            // for filter soft deleted objects
            modelBuilder.AddQueryFilters();

            #endregion /Model Configuration

            base.OnModelCreating(modelBuilder);

            //________________________________________________________________________________
        }
    }
}
