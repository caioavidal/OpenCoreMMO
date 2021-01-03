using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Repositories
{
    /// <summary>
    /// This class represents the generic repository base from application.
    /// </summary>
    /// <typeparam name="TEntity">Generic type for repository entity.</typeparam>
    public class BaseRepositoryNeo<TEntity, TContext> : IDisposable, IBaseRepositoryNeo<TEntity> where TEntity : class where TContext : DbContext
    {
        #region private members

        /// <summary>
        /// The generic dbset of entity.
        /// </summary>
        private readonly DbSet<TEntity> _entity;

        /// <summary>
        /// The context of database.
        /// </summary>
        private readonly DbContext DbContext;

        #endregion

        #region properties

        public TContext Context => (TContext)DbContext;

        #endregion

        #region constructors

        public BaseRepositoryNeo(DbContext dbContext)
        {
            DbContext = dbContext;
            _entity = DbContext.Set<TEntity>();
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// This method is responsible for insert generic entity in databse.
        /// </summary>
        /// <param name="entity">The generic entity to insert.</param>
        public async Task Insert(TEntity entity)
        {
            DbContext.Add(entity);
            await CommitChanges();
        }

        /// <summary>
        /// This method is responsible for update generic entity in databse.
        /// </summary>
        /// <param name="entity">The generic entity to update.</param>
        public async Task Update(TEntity entity)
        {
            DbContext.Update(entity);
            await CommitChanges();
        }

        /// <summary>
        /// This method is responsible for insert generic entity in databse.
        /// </summary>
        /// <param name="entity">The generic entity to insert.</param>
        public async Task Delete(TEntity entity)
        {
            DbContext.Remove(entity);
            await CommitChanges();
        }

        /// <summary>
        /// This method is responsible for get the query from entity.
        /// </summary>
        public IQueryable<TEntity> Query() => _entity.AsQueryable();

        /// <summary>
        /// This method is responsible for get all registers from entity table.
        /// </summary>
        public async Task<IList<TEntity>> GetAllAsync() => await _entity.ToListAsync();

        /// <summary>
        /// This method is responsible for dispose the database instance from application.
        /// </summary>
        public void Dispose()
        {
            DbContext.Dispose();
        }

        #endregion

        #region private methods implementation

        /// <summary>
        /// This method is responsible for save changes in database.
        /// </summary>
        /// <returns></returns>
        private async Task CommitChanges() => await DbContext?.SaveChangesAsync();

        #endregion
    }
}
