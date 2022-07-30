using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Contracts;
using Core.Application.Settings;
using Core.Domain.Contracts;

namespace Core.Application.Interfaces
{
    public interface IGenericSqlRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>, new()
    {
        #region Read Methods
        Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken, bool lazyLoading = true, bool withTracking = false, bool ignoreQueryFilters = false);
        //================================================================================
        Task<IDataResult<TEntity>> FilterAsync(
            PagingSetting? pagingSetting = null,
            string? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool withTracking = false,
            bool ignoreQueryFilters = false,
            params Expression<Func<TEntity, object>>[] includes);
        //================================================================================
        #endregion /Get Methods

        #region Insert Methods
        //================================================================================
        //void Insert(TEntity entity, bool saveChanges = true);
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken);
        //================================================================================
        //void InsertRange(IEnumerable<TEntity> entities, bool saveChanges = true);
        Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        //================================================================================
        #endregion /Insert Methods

        #region Update Methods
        //================================================================================
        void Update(TEntity entity);
        //================================================================================
        #endregion /Update Methods

        #region Delete Methods
        //================================================================================
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        //================================================================================
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        //================================================================================
        Task SoftDeleteAsync(TKey id, CancellationToken cancellationToken);
        #endregion /Delete Methods
        
    }
}
