using MongoDB.Bson;
using MongoDB.Driver;

namespace Coderaw.Settings.Stages
{
    public static class TenantStageFilter
    {
        public static FilterDefinition<BsonDocument> MatchByTenant(string tenantName)
        {
            var filter = new BsonDocument(
                "Tenant.Name",
                new BsonDocument("$eq", tenantName));

            return new BsonDocumentFilterDefinition<BsonDocument>(filter);
        }

        public static FilterDefinition<BsonDocument> MatchByDateInterval(
            DateTime dateIni,
            DateTime dateEnd,
            string fieldName)
        {
            if (dateIni == DateTime.MinValue || dateEnd == DateTime.MinValue)
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            var query = new BsonDocument(fieldName,
                new BsonDocument
                {
                    { "$gte", new DateTime(dateIni.Year, dateIni.Month, dateIni.Day, 0, 0, 0, DateTimeKind.Utc) },
                    { "$lte", new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59, DateTimeKind.Utc) },
                });

            return new BsonDocumentFilterDefinition<BsonDocument>(query);
        }
    }
}
