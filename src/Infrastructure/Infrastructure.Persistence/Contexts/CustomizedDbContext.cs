using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Domain.Contracts;
using Infrastructure.Common.Extensions;
using Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Persistence.Contexts
{
    public class CustomizedDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public CustomizedDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            
            Database.EnsureCreated();
        }

        //=====================================================================================================
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateDateTime = DateTime.UtcNow;
                        entry.Entity.CreatorIpAddress = _httpContextAccessor.HttpContext.GetIpAddress();
                        entry.Entity.CreatedBy = _httpContextAccessor.HttpContext.GetUserIdentifier();
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifyDateTime = DateTime.UtcNow;
                        entry.Entity.LastModifierIpAddress = _httpContextAccessor.HttpContext.GetIpAddress();
                        entry.Entity.LastModifiedBy = _httpContextAccessor.HttpContext.GetUserIdentifier();
                        break;
                }
            }

            return base.SaveChanges();
        }
        //=====================================================================================================
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        //=====================================================================================================
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateDateTime = DateTime.UtcNow;
                        entry.Entity.CreatorIpAddress = _httpContextAccessor.HttpContext.GetIpAddress();
                        entry.Entity.CreatedBy = _httpContextAccessor.HttpContext.GetUserIdentifier();
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifyDateTime = DateTime.UtcNow;
                        entry.Entity.LastModifierIpAddress = _httpContextAccessor.HttpContext.GetIpAddress();
                        entry.Entity.LastModifiedBy = _httpContextAccessor.HttpContext.GetUserIdentifier();
                        break;
                }
            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
        //=====================================================================================================
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        //=====================================================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //________________________________________________________________________________
            #region Model Configuration
            modelBuilder.RegisterAllEntities(Assembly.Load("Infrastructure.Persistence"));
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddPluralizingTableNameConvention();
            #endregion /Model Configuration

            //________________________________________________________________________________
        }
        //=====================================================================================================
    }
}
