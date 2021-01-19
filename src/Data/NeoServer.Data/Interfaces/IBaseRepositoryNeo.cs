using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoServer.Data.Interfaces
{
    public interface IBaseRepositoryNeo<TEntity> where TEntity : class
    {
        Task Insert(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task<IList<TEntity>> GetAllAsync();
        IQueryable<TEntity> Query();
    }
}
