using System.Linq.Expressions;

namespace Coderaw.Settings.Common
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task<long> ReplaceAsync(Expression<Func<TEntity, bool>> filterExpression,
            TEntity entity, CancellationToken cancellationToken);

        Task<long> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken);

        Task<TEntity> GetByIdAsync(Guid Id, CancellationToken cancellationToken);

        Task<TEntity> GetLatestBasedOnFieldAsync(string field, CancellationToken cancellationToken);

        Task<TEntity> GetFirstBasedOnFieldAsync(string field, CancellationToken cancellationToken);
    }
}
