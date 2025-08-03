using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Mattioli.Configurations.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly ILogger<BaseRepository<TEntity>> _logger;

        public BaseRepository(IMongoDatabase mongoDb, string collectionName, ILogger<BaseRepository<TEntity>> logger)
        {
            MapClasses();
            _collection = mongoDb.GetCollection<TEntity>(collectionName);
            _logger = logger;
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<long> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken)
        {
            var result = await _collection.DeleteOneAsync(filterExpression, cancellationToken);

            if (result.DeletedCount >= 1)
            {
                _logger.LogInformation("Document deleted with sucessfully on {@Entity}", filterExpression.Body.ToString());
            }

            return result.DeletedCount;
        }

        public async Task UpdateOneAsync(FilterDefinition<TEntity> filterDefinition,
            UpdateDefinition<TEntity> entity,
            UpdateOptions<TEntity> replaceOptions,
            CancellationToken cancellationToken)
        {
            await _collection.UpdateOneAsync(filterDefinition, entity, options: replaceOptions, cancellationToken: cancellationToken);
        }

        public async Task DeleteIfExistsAndInsertAsync(Expression<Func<TEntity, bool>> filterExpression, TEntity entity, CancellationToken cancellationToken)
        {
            await DeleteAsync(filterExpression, cancellationToken);
            await AddAsync(entity, cancellationToken);
        }

        public async Task<long> ReplaceAsync(Expression<Func<TEntity, bool>> filterExpression, TEntity entity, CancellationToken cancellationToken)
        {
            var result = await _collection.ReplaceOneAsync(filterExpression, entity, cancellationToken: cancellationToken);

            if (result.ModifiedCount >= 1)
            {
                _logger.LogInformation("Document updated with successfully {@Entity}", entity);
            }

            return result.ModifiedCount;
        }

        public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var result = await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<TEntity> GetLatestBasedOnFieldAsync(string field, CancellationToken cancellationToken)
        {
            var filter = Builders<TEntity>.Filter.Empty;
            var sort = Builders<TEntity>.Sort.Descending(field);

            var lastDocument = await _collection
                .Find(filter)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return lastDocument;
        }

        public async Task<TEntity> GetFirstBasedOnFieldAsync(string field, CancellationToken cancellationToken)
        {
            var filter = Builders<TEntity>.Filter.Empty;
            var sort = Builders<TEntity>.Sort.Ascending(field);

            var firstDocument = await _collection
                .Find(filter)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return firstDocument;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var results = await _collection.Find(predicate).ToListAsync(cancellationToken);
            return results;
        }

        private static void MapClasses()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
            {
                BsonClassMap.TryRegisterClassMap<TEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

    }
}
