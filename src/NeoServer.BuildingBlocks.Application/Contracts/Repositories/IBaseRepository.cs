namespace NeoServer.BuildingBlocks.Application.Contracts.Repositories;

public interface IBaseRepositoryNeo<TEntity> where TEntity : class
{
    Task Insert(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task<IList<TEntity>> GetAllAsync();
    Task<TEntity> GetAsync(int id);
}