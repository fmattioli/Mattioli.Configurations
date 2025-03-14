using Coderaw.Settings.Filters;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Coderaw.Settings.Extensions.Hangfire
{
    public static class HangFireExtensions
    {
        public static IServiceCollection AddHangFire(this IServiceCollection services, string connectionString, string dataBase)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            var mongoDataBase = dataBase;

            services.AddHangfire(config =>
            {
                config.UseMongoStorage(mongoClient, mongoDataBase, new MongoStorageOptions
                {
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo.metrics",
                    CheckConnection = true,
                });
            });

            services.AddHangfireServer();

            return services;
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, string dashboardUrl)
        {
            app.UseHangfireDashboard($"/{dashboardUrl}", new DashboardOptions
            {
                Authorization = [new AllowAllAuthorizationFilter()]
            });

            return app;
        }
    }
}
