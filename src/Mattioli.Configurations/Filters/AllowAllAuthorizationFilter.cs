using Hangfire.Dashboard;

namespace Mattioli.Configurations.Filters
{
    public class AllowAllAuthorizationFilter() : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
