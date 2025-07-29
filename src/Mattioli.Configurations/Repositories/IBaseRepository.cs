using MongoDB.Driver;
using System.Linq.Expressions;

namespace Mattioli.Configurations.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task<long> ReplaceAsync(Expression<Func<TEntity, bool>> filterExpression,
            TEntity entity, CancellationToken cancellationToken);

        Task<long> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken);

        Task DeleteIfExistsAndInsertAsync(Expression<Func<TEntity, bool>> filterExpression,
            TEntity entity, CancellationToken cancellationToken);

        Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<TEntity> GetLatestBasedOnFieldAsync(string field, CancellationToken cancellationToken);

        Task<TEntity> GetFirstBasedOnFieldAsync(string field, CancellationToken cancellationToken);

        Task UpdateOneAsync(FilterDefinition<TEntity> filterDefinition,
            UpdateDefinition<TEntity> entity,
            UpdateOptions<TEntity> replaceOptions,
            CancellationToken cancellationToken);
    }
}
