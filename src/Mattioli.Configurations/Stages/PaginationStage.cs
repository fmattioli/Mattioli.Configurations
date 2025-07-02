using MongoDB.Bson;
using MongoDB.Driver;

namespace Mattioli.Configurations.Stages
{
    public static class PaginationStage
    {
        public static PipelineDefinition<T, BsonDocument> Paginate<T>(
            this PipelineDefinition<T, BsonDocument> pipelineDefinition,
            int pageSize, int pageNumber) where T : class
        {

            var skipCount = pageSize * (pageNumber - 1);
            var pageSizeLimit = pageSize;

            return pipelineDefinition
                .Skip(skipCount)
                .Limit(pageSizeLimit);
        }
    }
}
