using MongoDB.Bson;
using MongoDB.Driver;

using System.Text.RegularExpressions;

namespace Coderaw.Settings.Stages
{
    public static class TenantStageFilter
    {
        public static FilterDefinition<BsonDocument> MatchByTenant(IEnumerable<string> tenantNames)
        {
            var names = tenantNames.Select(x => new BsonRegularExpression(new Regex(x, RegexOptions.IgnoreCase)));

            var filter = new BsonDocument(
                "Tenants.Name",
                new BsonDocument("$in", BsonArray.Create(names)));

            return new BsonDocumentFilterDefinition<BsonDocument>(filter);
        }
    }
}
