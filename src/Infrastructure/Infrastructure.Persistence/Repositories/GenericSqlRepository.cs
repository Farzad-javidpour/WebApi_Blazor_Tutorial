using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Application.Contracts;
using Core.Application.Interfaces;
using Core.Application.Settings;
using Core.Domain.Contracts;
using Infrastructure.Common.Extensions;
using Infrastructure.Common.Wrappers;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    //================================================================================
    public class GenericSqlRepository<TEntity, TKey> : IGenericSqlRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>, new()
    {
        #region Properties
        //================================================================================
        private readonly IMapper _mapper;
        internal CustomizedDbContext DataBaseContext;
        //================================================================================
        internal DbSet<TEntity> DbSet;
        //================================================================================
        #endregion /Properties

        #region Constructor
        //================================================================================
        public GenericSqlRepository(IMapper mapper, CustomizedDbContext databaseContext)
        {
            DataBaseContext = databaseContext ?? throw new ArgumentNullException(paramName: nameof(databaseContext));
            _mapper = mapper;
            //---------------------------
            DbSet = DataBaseContext.Set<TEntity>();
        }
        //================================================================================
        #endregion /Constructor

        #region Methods

        #region Get Methods
        //================================================================================
        public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool lazyLoading = true, bool withTracking = false, bool ignoreQueryFilters = false)
        {
            var query = DbSet.AsQueryable();

            if (lazyLoading == false)
            {
                var navigationList = DataBaseContext.Model.FindEntityType(typeof(TEntity))?
                    .GetDerivedTypesInclusive()
                    .SelectMany(type => type.GetNavigations())
                    .Distinct();

                if (navigationList != null)
                    foreach (var property in navigationList)
                        query = query.Include(property.Name);
            }

            if (withTracking == false) query = query.AsNoTracking();

            if (ignoreQueryFilters == true) query = query.IgnoreQueryFilters();

            var result = await query.SingleOrDefaultAsync(i => i.Id.Equals(id), cancellationToken);

            return result;
        }
        //================================================================================
        public async Task<IDataResult<TEntity>> FilterAsync(
           PagingSetting? pagingSetting = null,
           string? filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
           bool withTracking = false,
           bool ignoreQueryFilters = false,
           params Expression<Func<TEntity, object>>[] includes)
        {
            var query = DbSet.AsQueryable();
            var result = new DataResult<TEntity>();
            //------------------------------------------
            result.PaginationInfo = new PaginationInfo();
            if (pagingSetting != null)
            {
                result.PaginationInfo.CurrentPage = pagingSetting.PageIndex;
                result.PaginationInfo.TotalCount = query.Count();
                
            }
            //------------------------------------------
            if (filter != null)
            {
                query = query.Where(filter);
            }
            //------------------------------------------
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            //------------------------------------------
            if (pagingSetting != null && pagingSetting.PageSize > 0)
            {
                query = query
                    .Skip(pagingSetting.PageSize * (pagingSetting.PageIndex - 1))
                    .Take(pagingSetting.PageSize);
            }
            //------------------------------------------
            if (withTracking == false)
            {
                query = query.AsNoTracking();
            }
            //------------------------------------------
            if (ignoreQueryFilters == true)
            {
                query = query.IgnoreQueryFilters();
            }
            //------------------------------------------
            result.PaginationInfo.PageSize = query.Count();
            result.PaginationInfo.TotalPages = (int)Math.Ceiling(((double)result.PaginationInfo.TotalCount / (double)pagingSetting.PageSize));
            //------------------------------------------
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            //------------------------------------------
            result.Data = await query.ToListAsync();
            return result;
        }
        //================================================================================
        #endregion /Get Methods

        #region Insert Methods
        //================================================================================
        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }
        ////================================================================================
        public async Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }
        //================================================================================
        #endregion /Create Methods

        #region Update Methods
        //================================================================================
        public void Update(TEntity entity)
        {
            var entityProxy = DataBaseContext.Entry(entity);

            entityProxy.State = EntityState.Modified;
        }
        //================================================================================
        #endregion /Update Methods

        #region Delete Methods
        //================================================================================
        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity.IsDeleted == false)
            {
                await Task.Run(() => DbSet.Remove(entity), cancellationToken);

                return true;
            }
            else
            {
                return false;
            }
        }
        //================================================================================
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            var models = entities.Where(x => x.IsDeleted == false);
            await Task.Run(() => DbSet.RemoveRange(models), cancellationToken);
        }
        //================================================================================
        public async Task SoftDeleteAsync(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            //--------------------------
            entity.IsDeleted = true;
            //--------------------------
            Update(entity);
        }
        //================================================================================
        #endregion /Delete Methods

        public virtual void Attach(TEntity entity)
        {
            ArgumentAssert.NotNull(entity, nameof(entity));
            if (DataBaseContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);
        }
        public virtual void Detach(TEntity entity)
        {
            ArgumentAssert.NotNull(entity, nameof(entity));
            var entry = DataBaseContext.Entry(entity);
            if (entry != null)
                entry.State = EntityState.Detached;
        }

        #endregion /Methods
    }
}
