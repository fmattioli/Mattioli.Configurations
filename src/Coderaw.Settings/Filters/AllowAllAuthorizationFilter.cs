using Hangfire.Dashboard;

namespace Coderaw.Settings.Filters
{
    public class AllowAllAuthorizationFilter() : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
