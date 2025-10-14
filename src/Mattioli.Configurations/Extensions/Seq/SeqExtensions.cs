using Mattioli.Configurations.Models;
using Microsoft.AspNetCore.Builder;


namespace Mattioli.Configurations.Extensions.Sentry
{
    public static class SeqExtensions
    {
        public static WebApplicationBuilder UseMltSeq(this WebApplicationBuilder builder, MltSettings mltSettings)
        {
            if (string.IsNullOrWhiteSpace(mltSettings.Dsn))
            {
                return builder;
            }

            return builder;
        }
    }
}
